using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LiteDB;

namespace ToneAudioPlayer.DataSources.Local;

public class LocalDataSource: IDataSource
{
    public string Id /*{ get; }*/ => "Local";

    private readonly ILocalDataSourceSettings _settings;
    private readonly ILiteDatabase _db;

    private readonly string _filePath;


    public LocalDataSource(ILocalDataSourceSettings settings, ILiteDatabase db)
    {
        _settings = settings;
        _filePath = Path.Combine(settings.StorageFolder, "files");
        _db = db;
    }

    private void EnsureDirectoriesExist()
    {
        if (!Directory.Exists(_filePath))
        {
            Directory.CreateDirectory(_filePath);
        }
    }
    
    public Task<List<DataSourceItem>> SearchAsync(string q)
    {
        throw new NotImplementedException();
    }

    public Task<DataSourceItem?> GetItemByIdAsync(string id)
    {
        throw new NotImplementedException();
    }

    public void HandleAction(DataSourceItem item, DataSourceAction action, object? context)
    {
        switch (action)
        {
            case DataSourceAction.Play:
                
                break;
        }
    }

    private string BuildFullPath(DataSourceItem item, DataSourceMediaItem mediaItem)
    {
        return Path.Combine(BuildStoragePath(item), BuildStorageFileName(mediaItem));
    }
    
    
    private string BuildStoragePath(DataSourceItem searchResult)
    {
        var series = searchResult.Movements.FirstOrDefault()?.Name.Trim();
        var authors = string.Join(", ", searchResult.Artists.Select(a => a.Name.Trim()));
        var baseSubDirectory = searchResult.Movements.Count > 0 ? "individuals" : "series";
        var genrePath = searchResult.Genres.Count > 0 ? searchResult.Genres.First() : "Misc";
        var seriesPath = string.IsNullOrEmpty(series) ? "" : series;
        var pathParts = new List<string>
        {
            baseSubDirectory,
            genrePath,
            authors, 
            seriesPath,
            searchResult.Title
        }.Where(p => !string.IsNullOrEmpty(p)).Select(ReplaceInvalidChars);

        return Path.Combine(_filePath, baseSubDirectory, string.Join(Path.DirectorySeparatorChar, pathParts));

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

*/

    private static string BuildStorageFileName(DataSourceMediaItem item)
    {
        var rawFilename = item.FileName;
        if (item.Extension.Length > 0 && item.FileName.EndsWith(item.Extension))
        {
            rawFilename = item.FileName[..^item.Extension.Length];
        }
        

        return $"{rawFilename}.{item.Id}.{item.Extension}";
        
    }
    
    private static string ReplaceInvalidChars(string filename)
    {
        return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));    
    }
    
}