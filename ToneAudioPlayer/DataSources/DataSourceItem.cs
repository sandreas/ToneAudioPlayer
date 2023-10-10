using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ToneAudioPlayer.DataSources;

public class DataSourceItem
{
    public string Id { get; set; }
    public List<DataSourceMediaItem> MediaItems;
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Publisher { get; set; } = "";
    
    public DataSourceItemStatus Status { get; set; } = DataSourceItemStatus.Paused;
    public List<string> Genres { get; set; } = new();
    public List<DataSourcePerson> Artists { get; set; } = new();
    public List<DataSourcePerson> Composers { get; set; } = new();
    public List<DataSourceMovement> Movements { get; set; } = new();
    public List<Stream> Covers { get; set; } = new();

    public bool HasProgress => MediaItemIndex > 0 || MediaItems.Any(m => m.Position > TimeSpan.Zero);
    
    public int MediaItemIndex = 0;
    public DataSourceMediaItem? ActiveMediaItem => MediaItems.Count >= MediaItemIndex
        ? MediaItems.ElementAtOrDefault(MediaItemIndex)
        : MediaItems.FirstOrDefault();
    
    public TimeSpan TotalPosition => MediaItems.Count == 0 ? TimeSpan.Zero : TimeSpan.FromMilliseconds(MediaItems.Take(MediaItemIndex).Sum(i => i.Duration.TotalMilliseconds)) + ActiveMediaItem?.Position ?? TimeSpan.Zero;
    public TimeSpan TotalDuration => TimeSpan.FromMilliseconds(MediaItems.Sum(i => i.Duration.TotalMilliseconds));
    // public TimeSpan Position => MediaItems.Count <= MediaItemIndex ? TimeSpan.Zero : TimeSpan.FromMilliseconds(MediaItems.Sum(i => i.Duration.TotalMilliseconds));
    
    public DataSourceItem(string id, List<DataSourceMediaItem> mediaItems)
    {
        Id = id;
        MediaItems = mediaItems;
    }
    
    
    

}