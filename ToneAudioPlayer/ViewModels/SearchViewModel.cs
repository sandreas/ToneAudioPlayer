using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AudiobookshelfApi;
using AudiobookshelfApi.Models;
using AudiobookshelfApi.Responses;
using Avalonia.Collections;
using Avalonia.SimpleRouter;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibVLCSharp.Shared;
using ToneAudioPlayer.Services;
using ToneAudioPlayer.ViewModels.Search;
using Media = AudiobookshelfApi.Models.Media;

namespace ToneAudioPlayer.ViewModels;

public partial class SearchViewModel: ViewModelBase
{
    private readonly Audiobookshelf _abs;

    private Library? _selectedLibrary;
    
    [ObservableProperty] 
    private string _query = "";

    [ObservableProperty]
    private AvaloniaList<SearchResultViewModel> _searchResults = new();

    private readonly MediaPlayerService _player;

    public SearchViewModel(Audiobookshelf abs, MediaPlayerService player)
    {
        _abs = abs;
        _player = player;
    }

    [RelayCommand]
    private async void Search(string q)
    {
        
        if (_selectedLibrary == null)
        {
            var response = await _abs.GetLibrariesAsync();
            if (response is LibrariesResponse lr)
            {
                _selectedLibrary = lr.Libraries.FirstOrDefault();
            }
        }

        if (_selectedLibrary == null)
        {
            return;
        }

        var searchResponse = await _abs.SearchLibraryAsync(_selectedLibrary.Id, q);

        if (searchResponse is SearchLibraryResponse sr)
        {
            SearchResults.Clear();
            SearchResults.AddRange(
                sr.Book
                    .Select(b => new SearchResultViewModel
                    {
                        Title = b.LibraryItem.Media.Metadata.Title,
                        LibraryItem = b.LibraryItem
                    })
                );
        }
        
    }

    [RelayCommand]
    private async void Play(SearchResultViewModel searchResult)
    {
        // Todo:
        // https://api.audiobookshelf.org/#get-a-media-progress
        var progressResponse = await _abs.GetMediaProgress(searchResult.LibraryItem.Id);

        var currentTime = TimeSpan.Zero;
        if (progressResponse is MediaProgressResponse mpr)
        {
            currentTime = TimeSpan.FromSeconds(mpr.CurrentTime);
        }
        
        _player.UpdateMedia(_abs.BuildLibraryItemUrl(searchResult.LibraryItem));
        _player.Play();
        _player.SeekTo(currentTime);

    }
}