using System.Threading.Tasks;
using Avalonia.Collections;
using Avalonia.SimpleRouter;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ToneAudioPlayer.DataSources;
using ToneAudioPlayer.ViewModels.Search;

namespace ToneAudioPlayer.ViewModels;

public partial class HomeViewModel: ViewModelBase
{
    private readonly HistoryRouter<ViewModelBase> _router;
    private readonly AudiobookshelfDataSource _dataSource;

    [ObservableProperty] private string _query = "";
    [ObservableProperty] private AvaloniaList<SearchResultViewModel> _searchResults = new();

    public HomeViewModel(HistoryRouter<ViewModelBase> router, AudiobookshelfDataSource dataSource)
    {
        _router = router;
        _dataSource = dataSource;
    }
    
    [RelayCommand]
    private async Task Search(string q)
    {
        var dataSourceResults = await _dataSource.SearchAsync(q);
        SearchResults.Clear();
        SearchResults.AddRange(dataSourceResults);
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
}