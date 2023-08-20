namespace AudiobookshelfApi.Models;

public class AudioFile
{
    public int Index { get; set; }
    public string Ino { get; set; } = "";
    public List<Chapter> Chapters { get; set; } = new();
}