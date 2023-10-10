using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AudiobookshelfApi.Models;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using Material.Icons;
using ToneAudioPlayer.DataSources;

namespace ToneAudioPlayer.ViewModels.Search;

public partial class SearchResultViewModel: ViewModelBase
{
    [ObservableProperty] private DataSourceItem _item;
    
    [ObservableProperty] private string _title = "";
    [ObservableProperty] private string _description = "";
    [ObservableProperty] private string _authors = "";
    [ObservableProperty] private string _narrators = "";
    [ObservableProperty] private string _series = "";
    [ObservableProperty] private string _genre = "";
    [ObservableProperty] private TimeSpan _position = TimeSpan.Zero;
    [ObservableProperty] private TimeSpan _duration = TimeSpan.Zero;
    
    public Bitmap? FirstCover => Item.Covers.Count > 0 ? new Bitmap(Item.Covers.First()) : null;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(DownloadPercent))]private float _downloadProgress;

    public string DownloadPercent => DownloadProgress.ToString("P");
    

    public MaterialIconKind ToggleIcon => (Item.Status == DataSourceItemStatus.Playing) ? MaterialIconKind.Pause : (Item.HasProgress ? MaterialIconKind.Resume : MaterialIconKind.Play);


    public SearchResultViewModel(DataSourceItem item)
    {
        Item = item;
    }

    public void NotifyPropertyChanged(string propertyName)
    {
        OnPropertyChanged(propertyName);
    }
}