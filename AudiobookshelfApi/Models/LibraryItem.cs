using AudiobookshelfApi.Responses;

namespace AudiobookshelfApi.Models;

public class LibraryItem
{
    public string Id { get; set; } = "";
    public string Ino { get; set; } = "";
    public string RelPath { get; set; } = "";
    public Media Media { get; set; } = new();
    public MediaProgress UserMediaProgress { get; set; } = new();
    public List<LibraryFile> LibraryFiles { get; set; } = new();
    public List<AudioFile> AudioFiles { get; set; } = new();

    
}