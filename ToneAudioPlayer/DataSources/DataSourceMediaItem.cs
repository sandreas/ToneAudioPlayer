
using System;
using System.Text.Json.Serialization;

namespace ToneAudioPlayer.DataSources;

public class DataSourceMediaItem
{
    public string Id  { get; set; } = "";
    public string FileName { get; set; } = "";
    public string Extension { get; set; } = "";
    public Uri Url { get; set; } = new("about:blank");

    public TimeSpan Duration { get; set; } = TimeSpan.Zero;
    public TimeSpan Position { get; set; } = TimeSpan.Zero;

}