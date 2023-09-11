using System;
using System.Threading.Tasks;
using AudiobookshelfApi.Responses;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediaManager;
using MediaManager.Playback;
using ToneAudioPlayer.DataSources;
using ToneAudioPlayer.Services;
using ToneAudioPlayer.ViewModels.Search;

namespace ToneAudioPlayer.ViewModels;

public partial class SearchViewModel : ViewModelBase
{
    private readonly MediaPlayerService _player;
    private readonly AudiobookshelfDataSource _dataSource;

    [ObservableProperty] private string _query = "";

    [ObservableProperty] private AvaloniaList<SearchResultViewModel> _searchResults = new();

    [ObservableProperty] private string _logs = "";

    public SearchViewModel(MediaPlayerService player, AudiobookshelfDataSource dataSource)
    {
        _player = player;
        _dataSource = dataSource;
        _player.StateChanged += OnStateChange;
        
    }

    private void OnStateChange(object sender, StateChangedEventArgs e)
    {
        Logs = e.State + "\n" + Logs;
    }

    [RelayCommand]
    private async Task Search(string q)
    {
        var dataSourceResults = await _dataSource.SearchAsync(q);
        SearchResults.Clear();
        SearchResults.AddRange(dataSourceResults);
    }

    [RelayCommand]
    private void StartPlayback(IItemIdentifier identifier)
    {
        _player.ResumeMedia(identifier);
    }

    [RelayCommand]
    private void Prev()
    {
        _player.PreviousChapter();
    }

    [RelayCommand]
    private void Next()
    {
        _player.NextChapter();
    }

    [RelayCommand]
    private void Pause()
    {
        _player.Pause();
    }

    [RelayCommand]
    private void Play()
    {
        _player.Play();
    }

    [RelayCommand]
    private void SeekBack()
    {
        _player.Seek(TimeSpan.FromSeconds(-30));
    }

    [RelayCommand]
    private void SeekForward()
    {
        _player.Seek(TimeSpan.FromSeconds(30));
    }
}