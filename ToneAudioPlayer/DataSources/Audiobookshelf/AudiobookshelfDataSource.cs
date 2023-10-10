using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AudiobookshelfApi.Models;
using AudiobookshelfApi.Requests;
using AudiobookshelfApi.ResponseModels;
using AudiobookshelfApi.Responses;
using ToneAudioPlayer.DataSources.Local;

namespace ToneAudioPlayer.DataSources.Audiobookshelf;

public class AudiobookshelfDataSource: IDataSource
{
    public string Id /*{ get; }*/ => "Audiobookshelf";

    private readonly AudiobookshelfApi.Audiobookshelf _abs;

    private readonly Dictionary<string, TimeSpan> _progressStorage = new();
    private Library? _selectedLibrary;
    private readonly LocalDataSource _local;
    
    
    public AudiobookshelfDataSource(AudiobookshelfApi.Audiobookshelf abs, LocalDataSource local)
    {
        _abs = abs;
        _local = local;
    }

    
    private async Task Init()
    {
        if (_selectedLibrary != null)
        {
            return;
        }
        var response = await _abs.GetLibrariesAsync();
        if (response is Response<LibrariesResponse> lr)
        {
            _selectedLibrary = lr.Value.Libraries.FirstOrDefault();
        }

    }
    
    private async Task<bool> UpdateProgressAsync(DataSourceItem item, TimeSpan currentPosition, TimeSpan totalLength)
    {
        await Init();
        
        var lastProgress = _progressStorage.TryGetValue(item.Id, out var value) ? value : TimeSpan.MinValue;
        
         // no update required until difference is at least 10 seconds
         if (currentPosition <= TimeSpan.Zero || Math.Abs(currentPosition.TotalMilliseconds - lastProgress.TotalMilliseconds) < 10000)
         {
             return true;
         }

        var progressAsFloat = 100 / totalLength.TotalMilliseconds * currentPosition.TotalMilliseconds;
        _progressStorage[item.Id] = currentPosition;
        
        var progressRequest = new MediaProgressRequest()
        {
            CurrentTime = currentPosition.TotalSeconds,
            Progress = progressAsFloat,
            IsFinished = totalLength - currentPosition < TimeSpan.FromSeconds(10)
        };
        var response = await _abs.UpdateMediaProgressAsync(progressRequest, item.Id);
        
        var success = response is Response<MediaProgress>;
        // Debug.WriteLine($"currentTime: {seconds}, progress: {progress}, success: {success}");
        return success;
    }
    

    private async Task<TimeSpan> GetMediaProgressAsync(string itemId, string? episodeId=null,
        CancellationToken? cancellationToken = null)
    {
        
        await Init();
        
        var currentTime = TimeSpan.Zero;
        var progressResponse = await _abs.GetMediaProgressAsync(itemId, episodeId, cancellationToken);
        if (progressResponse is Response<MediaProgress> mpr)
        {
            currentTime = TimeSpan.FromSeconds(mpr.Value.CurrentTime);
        }

        return currentTime;
    }
    
    public async Task<DataSourceItem?> GetItemByIdAsync(string id)
    {
        await Init();
        if (_selectedLibrary == null)
        {
            return null;
        }
        var item = await _abs.GetLibraryItem(id);
        if (item is not Response<LibraryItem> lir)
        {
            return null;
        }

        return await LibraryItemToDataSourceItemAsync(lir.Value);
    }


    public async Task<List<DataSourceItem>> SearchAsync(string q)
    {
        await Init();
        if (_selectedLibrary == null)
        {
            return new List<DataSourceItem>();
        }

        var searchResponse = await _abs.SearchLibraryAsync(_selectedLibrary.Id, q);

        if (searchResponse is not Response<SearchLibraryResponse> sr)
        {
            return new List<DataSourceItem>();
        }
        
        var dataSourceItems = await Task.WhenAll(sr.Value.Book
            .Select(b => LibraryItemToDataSourceItemAsync(b.LibraryItem)));
            
        foreach ( var item in dataSourceItems)
        {
            var stream = await _abs.LoadLibraryItemCover(item.Id);
            if (stream != null)
            {
                item.Covers = new List<Stream>()
                {
                    stream
                };
            }

            
            
        }

        return dataSourceItems.ToList();

    }

    private async Task<DataSourceItem> LibraryItemToDataSourceItemAsync(LibraryItem item)
    {
        var currentPosition = await GetMediaProgressAsync(item.Id);
        var mediaItems = BuildMediaItems(item, currentPosition);
        
        
        
        return new DataSourceItem(item.Id, mediaItems)
        {
            Title = item.Media.Metadata.Title,
            Description = item.Media.Metadata.Description,
            Genres = item.Media.Metadata.Genres.ToList(),
            Publisher = item.Media.Metadata.Publisher,
            Movements = item.Media.Metadata.Series.Select(s => new DataSourceMovement
            {
                Id = s.Id,
                Name = s.Name,
                Sequence = s.Sequence
            }).ToList(),
            Artists = item.Media.Metadata.Authors.Select(a => new DataSourcePerson
            {
                Id = a.Id,
                Name = a.Name
            }).ToList(),
            Composers = item.Media.Metadata.Narrators.Select(n => new DataSourcePerson
            {
                Name = n
            }).ToList()
            
        };
    }

    private List<DataSourceMediaItem> BuildMediaItems(LibraryItem item, TimeSpan currentPosition)
    {
        /*
var parts = f.TimeBase
    .Split("/")
    .Select(s => float.TryParse(s, out var i) ? i : 0)
    .Where(i => i > 0)
    .ToArray();

var factor = 1.0f / 1000;
if (parts.Length == 2)
{
    factor = parts[0] / parts[1];
}
*/
        // https://abs.example.com/api/me/progress/li_bufnnmp4y5o2gbbxfm/ep_lh6ko39pumnrma3dhv


        var lengthSoFar = TimeSpan.Zero;
        return item.Media.AudioFiles.Select(f =>
            {
                var itemDuration = TimeSpan.FromSeconds(f.Duration ?? 0);  // TimeBase seems not be be considered
                var itemPosition = TimeSpan.Zero; // maybe TimeSpan.MinValue would be better, because an item can have 0 as position and be active?
                
                lengthSoFar += itemDuration;
                if (lengthSoFar > currentPosition)
                {
                    itemPosition = lengthSoFar - itemDuration + currentPosition;
                }
                return new DataSourceMediaItem
                {
                    Id = f.Ino.ToString(),
                    Url = _abs.BuildMediaUrl(item.Id,
                        f.Ino.ToString()), // new Uri($"{_abs.BaseAddress?.ToString().TrimEnd('/')}/api/items/{item.Id}/file/{f.Ino}?token={_credentials.Token}"),
                    FileName = f.Metadata.Filename,
                    Extension = f.Metadata.Ext, // including .
                    Position = itemPosition, // todo: calculate position and index
                    // todo: active? instead of index
                    Duration = itemDuration
                    // OriginalPath = f.Metadata.Path
                };
            })
            .ToList();

    }

    public void HandleAction(DataSourceItem? item, DataSourceAction action, object? context=null)
    {
        switch (action)
        {
            case DataSourceAction.Play:
                UpdateItemStatus(item, DataSourceItemStatus.Playing);
                break;
            case DataSourceAction.Pause:
                UpdateItemStatus(item, DataSourceItemStatus.Paused);
                break;
            case DataSourceAction.MediaItemChanged:
                if (item != null && context is DataSourceMediaItem mediaItem)
                {
                    item.MediaItemIndex = Math.Max(item.MediaItems.IndexOf(mediaItem), 0);
                }
                break;
            case DataSourceAction.PositionChanged:
                if (item?.ActiveMediaItem != null && context is TimeSpan position)
                {
                    item.ActiveMediaItem.Position = position;
                }
                break;
        }

        if (item != null)
        {
            _ = UpdateProgressAsync(item, item.TotalPosition, item.TotalDuration);
        }
    }
    
    private void UpdateItemStatus(DataSourceItem? item, DataSourceItemStatus status)
    {
        if (item != null)
        {
            item.Status = status;
        }
    }
    
    

    /*
    public DownloadPackage CreateDownloadPackage(DataSourceItem searchResult, object? context)
    {
        var downloadItems = searchResult.MediaItems;
        var storageUrl = new Uri(_settings.StorageFolder);

        var destinationPath = BuildPath(storageUrl.LocalPath, searchResult);
        if (!Directory.Exists(destinationPath))
        {
            Directory.CreateDirectory(destinationPath);
        }

        var downloadPackage = new DownloadPackage(searchResult.Id, context);
        
        foreach (var mediaItem in downloadItems)
        {
            var destinationFile = Path.Combine(destinationPath, BuildFileName(mediaItem));
            var destinationUrl = new Uri(destinationFile);
            var id = mediaItem.Url.ToString();
            var download = new Download(id, downloadPackage, mediaItem.Url, destinationUrl);
            downloadPackage.Downloads.Add(download);
        }

        return downloadPackage;
        // _downloader.Download(downloadPackage);
    }

    private static string BuildPath(string storageUrlPath, DataSourceItem searchResult)
    {
        var series = searchResult.Movements.FirstOrDefault()?.Name.Trim();
        var authors = string.Join(", ", searchResult.Artists.Select(a => a.Name.Trim()));
        var basePath = searchResult.Movements.Count > 0 ? "individuals" : "series";
        var genrePath = searchResult.Genres.Count > 0 ? searchResult.Genres.First() : "Misc";
        var seriesPath = string.IsNullOrEmpty(series) ? "" : series;
        var pathParts = new List<string>()
        {
            basePath,
            genrePath,
            authors, 
            seriesPath,
            searchResult.Title
        }.Where(p => !string.IsNullOrEmpty(p)).Select(ReplaceInvalidChars);

        return Path.Combine(storageUrlPath, basePath, string.Join(Path.DirectorySeparatorChar, pathParts));
    }



    private static string BuildFileName(DataSourceMediaItem item)
    {
        var rawFilename = item.FileName;
        if (item.Extension.Length > 0 && item.FileName.EndsWith(item.Extension))
        {
            rawFilename = item.FileName[..^item.Extension.Length];
        }
        

        return $"{rawFilename}{item.Id}{item.Extension}";
        
    }
    
    private static string ReplaceInvalidChars(string filename)
    {
        return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));    
    }
    */
}

