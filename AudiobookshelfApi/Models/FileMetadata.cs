namespace AudiobookshelfApi.Models;

public class FileMetadata
{
    public string Filename { get; set; } = "";
    public string Ext { get; set; } = "";
    public long Size  { get; set; } = 0;
    public string Path { get; set; } = "";

}