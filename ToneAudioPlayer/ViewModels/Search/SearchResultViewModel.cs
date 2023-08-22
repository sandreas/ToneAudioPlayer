using AudiobookshelfApi.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ToneAudioPlayer.ViewModels.Search;

public partial class SearchResultViewModel: ViewModelBase
{
    [ObservableProperty] private string _title = "";
    [ObservableProperty] private LibraryItem _libraryItem = new();

}