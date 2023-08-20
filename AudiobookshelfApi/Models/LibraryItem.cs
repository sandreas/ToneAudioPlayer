namespace AudiobookshelfApi.Models;

public class LibraryItem
{
    public string Id { get; set; } = "";
    public string Ino { get; set; } = "";
    public string RelPath { get; set; } = "";
    public Media Media { get; set; } = new();
}