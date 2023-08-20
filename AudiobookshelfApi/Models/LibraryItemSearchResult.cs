namespace AudiobookshelfApi.Models;

public class LibraryItemSearchResult
{
    public LibraryItem LibraryItem { get; set; } = new();
    public string MatchKey { get; set; } = "";
    public string MatchText { get; set; } = "";
}