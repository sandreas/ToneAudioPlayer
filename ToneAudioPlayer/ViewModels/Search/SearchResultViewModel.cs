using AudiobookshelfApi.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using ToneAudioPlayer.DataSources;

namespace ToneAudioPlayer.ViewModels.Search;

public partial class SearchResultViewModel: ViewModelBase
{
    [ObservableProperty] private AudiobookshelfItemIdentifier _identifier = new();
    [ObservableProperty] private LibraryItem _item = new();
    [ObservableProperty] private string _title = "";


}