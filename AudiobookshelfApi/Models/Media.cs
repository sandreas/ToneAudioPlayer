namespace AudiobookshelfApi.Models;

public class Media
{
    public Metadata Metadata { get; set; } = new();
    public List<AudioFile> AudioFiles { get; set; } = new();

}