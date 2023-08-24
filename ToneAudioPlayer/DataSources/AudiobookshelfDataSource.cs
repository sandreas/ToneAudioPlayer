using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AudiobookshelfApi.Models;
using AudiobookshelfApi.Requests;
using AudiobookshelfApi.Responses;
using ToneAudioPlayer.ViewModels.Search;

namespace ToneAudioPlayer.DataSources;

public class AudiobookshelfDataSource
{
    private readonly AudiobookshelfApi.Audiobookshelf _abs;

    private readonly Dictionary<string, float> _progressStorage = new();
    private readonly IAudiobookshelfSettings _settings;
    private Library? _selectedLibrary;

    public AudiobookshelfDataSource(AudiobookshelfApi.Audiobookshelf abs, IAudiobookshelfSettings settings)
    {
        _abs = abs;
        _settings = settings;
    }

    private async Task Init()
    {
        if (_selectedLibrary != null)
        {
            return;
        }
        var response = await _abs.GetLibrariesAsync();
        if (response is LibrariesResponse lr)
        {
            _selectedLibrary = lr.Libraries.FirstOrDefault();
        }

    }
    
    public async Task<bool> UpdateProgressAsync(IItemIdentifier id, float seconds, float totalSeconds)
    {
        await Init();
        if (id is not AudiobookshelfItemIdentifier absIdentifier)
        {
            return false;
        }
        /*
 var lastProgress = _progressStorage.TryGetValue(absIdentifier.Id, out var value) ? value : float.MinValue;
 

 // no update required until difference is at least 60 seconds
 if (seconds == 0 || Math.Abs(lastProgress - seconds) < 60)
 {
     return true;
 }
 */
        // totalSeconds = 100%
        // seconds = ?%
        
        // totalSeconds = 3500
        // seconds = 18

        var progress = 100 / totalSeconds * seconds;
        _progressStorage[absIdentifier.Id] = seconds;
        
        var progressRequest = new MediaProgressRequest()
        {
            // CurrentTime = seconds,
            Progress = progress,
            IsFinished = totalSeconds - seconds < 10
        };
        var response = await _abs.UpdateMediaProgressAsync(progressRequest, absIdentifier.Id);
        return response is MediaProgressResponse;
    }

    public async Task<AbstractResponse> GetMediaProgressAsync(IItemIdentifier libraryItemId,
        CancellationToken? cancellationToken = null)
    {
        if (libraryItemId is not AudiobookshelfItemIdentifier identifier)
        {
            return new ErrorResponse();
        }
        await Init();
        
        return await _abs.GetMediaProgressAsync(identifier.Id, null, cancellationToken);
    }
    
    public async Task<List<SearchResultViewModel>> SearchAsync(string q)
    {
        await Init();
        if (_selectedLibrary == null)
        {
            return new List<SearchResultViewModel>();
        }

        var searchResponse = await _abs.SearchLibraryAsync(_selectedLibrary.Id, q);

        if (searchResponse is SearchLibraryResponse sr)
        {
            return sr.Book
                .Select(b => new SearchResultViewModel
                {
                    Identifier = new AudiobookshelfItemIdentifier()
                    {
                        Id = b.LibraryItem.Id,
                        MediaUrl = _abs.BuildLibraryItemUrl(b.LibraryItem)
                    },
                    Item = b.LibraryItem,
                    Title = b.LibraryItem.Media.Metadata.Title,
                    
                }).ToList();
        }
        return new List<SearchResultViewModel>();
        
    }
}

