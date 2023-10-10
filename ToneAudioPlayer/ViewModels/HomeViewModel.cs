using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Collections;
using Avalonia.Platform.Storage;
using Avalonia.SimpleRouter;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ToneAudioPlayer.DataSources;
using ToneAudioPlayer.DataSources.Audiobookshelf;
using ToneAudioPlayer.Services;
using ToneAudioPlayer.ViewModels.Search;

namespace ToneAudioPlayer.ViewModels;

public partial class HomeViewModel: ViewModelBase
{
    private readonly IStorageProvider _storage;
    private readonly HistoryRouter<ViewModelBase> _router;
    private readonly AudiobookshelfDataSource _dataSource;

    [ObservableProperty] private string _query = "";
    [ObservableProperty] private AvaloniaList<SearchResultViewModel> _searchResults = new();
    
    private readonly CancellationTokenSource _cts;

    private DateTime _lastProgressReport = DateTime.MinValue;
    private readonly MediaPlayerService _player;

    public HomeViewModel(MediaPlayerService player, IStorageProvider storage, HistoryRouter<ViewModelBase> router, AudiobookshelfDataSource dataSource)
    {
        _storage = storage;
        _router = router;
        _dataSource = dataSource;
        _cts = new CancellationTokenSource();
        _player = player;
    }
    
    [RelayCommand]
    private async Task Search(string q)
    {
        var dataSourceResults = await _dataSource.SearchAsync(q);
        SearchResults.Clear();
        SearchResults.AddRange(dataSourceResults.Select(DataSourceItemToSearchResultViewModel));
    }

    private SearchResultViewModel DataSourceItemToSearchResultViewModel(DataSourceItem dataSourceItem)
    {
        return new SearchResultViewModel(dataSourceItem)
        {
            Title = dataSourceItem.Title,
            Description = dataSourceItem.Description,
            Series = dataSourceItem.Movements.FirstOrDefault()?.Name ?? "",
            Position = dataSourceItem.TotalPosition,
            Duration = dataSourceItem.TotalDuration
        };
    }        


    [RelayCommand]
    private void Navigate(string parameter = "")
    {
        switch (parameter)
        {
            case "music":
                // _router.GoTo<>();
                break;
            case "settings":
                _router.GoTo<SettingsViewModel>();
                break;            
            case "search":
                _router.GoTo<SearchViewModel>();
                break;
        }
    }

    [RelayCommand]
    private void Toggle(object parameter)
    {
        if (parameter is not SearchResultViewModel searchResult)
        {
            return;
        }
        
        _ = Task.Run(async () =>
        {
            await _player.UpdateItemAsync(searchResult.Item);
            await _player.ToggleAsync();
            searchResult.NotifyPropertyChanged(nameof(searchResult.ToggleIcon));
        });
        // _dataSource.CreateDownloadPackage(searchResult.Item, searchResult, new Progress<DownloadPackage>(HandleProgress), _cts.Token);
    }
    
    /*
    [RelayCommand]
    private void Download(object parameter)
    {
        if (parameter is not SearchResultViewModel searchResult)
        {
            return;
        }

        // _dataSource.CreateDownloadPackage(searchResult.Item, searchResult, new Progress<DownloadPackage>(HandleProgress), _cts.Token);
    }
    */

    private  void HandleProgress(DownloadPackage downloadPackage)
    {
        if (downloadPackage.Progress < 1 && _lastProgressReport  > DateTime.Now - TimeSpan.FromSeconds(1))
        {
            return;
        }
        
        if (downloadPackage.Context is not SearchResultViewModel r)
        {
            return;
        }
        
        r.DownloadProgress = downloadPackage.Progress;
        _lastProgressReport = DateTime.Now;
    }
}