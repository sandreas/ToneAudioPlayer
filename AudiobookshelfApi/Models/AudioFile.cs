using System.Text.Json.Serialization;

namespace AudiobookshelfApi.Models;

public class AudioFile
{
    public int Index { get; set; }
    public string Ino { get; set; } = "";
    public FileMetadata Metadata { get; set; } = new();
    public List<Chapter> Chapters { get; set; } = new();
    
    public double? Duration { get; set; }
    public string TimeBase { get; set; } = "1/1000";
    
    
}