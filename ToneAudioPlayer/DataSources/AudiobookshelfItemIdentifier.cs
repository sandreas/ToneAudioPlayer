using System;

namespace ToneAudioPlayer.DataSources;

public class AudiobookshelfItemIdentifier: IItemIdentifier
{
    public string Id { get; set; } = "";
    public string[] MediaUrls { get; set; } = Array.Empty<string>(); 
}