using System;
using System.Linq;
using AudiobookshelfApi;
using AudiobookshelfApi.Models;
using AudiobookshelfApi.Responses;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ToneAudioPlayer.DataSources;
using ToneAudioPlayer.Services;
using ToneAudioPlayer.ViewModels.Search;

namespace ToneAudioPlayer.ViewModels;

public partial class SearchViewModel: ViewModelBase
{
    private readonly AudiobookshelfDataSource _dataSource;
    
    [ObservableProperty] 
    private string _query = "";

    [ObservableProperty]
    private AvaloniaList<SearchResultViewModel> _searchResults = new();

    private readonly MediaPlayerService _player;

    public SearchViewModel(AudiobookshelfDataSource dataSource, MediaPlayerService player)
    {
        _dataSource = dataSource;
        _player = player;
    }

    [RelayCommand]
    private async void Search(string q)
    {
        var dataSourceResults = await _dataSource.SearchAsync(q);
        SearchResults.Clear();
        SearchResults.AddRange(dataSourceResults);
    }

    [RelayCommand]
    private async void Play(IItemIdentifier identifier)
    {
        // Todo:
        // https://api.audiobookshelf.org/#get-a-media-progress
        var progressResponse = await _dataSource.GetMediaProgressAsync(identifier);

        var currentTime = TimeSpan.Zero;
        if (progressResponse is MediaProgressResponse mpr)
        {
            currentTime = TimeSpan.FromSeconds(mpr.CurrentTime);
        }
        
        await _player.UpdateMediaAsync(identifier);
        /*
        _player.Play();
        _player.SeekTo(currentTime);
        */
        _player.SeekToAndPlay(currentTime);
    }
}