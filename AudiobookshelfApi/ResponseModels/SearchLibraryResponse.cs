using AudiobookshelfApi.Models;

namespace AudiobookshelfApi.ResponseModels;

public class SearchLibraryResponse
{
    public List<LibraryItemSearchResult> Book { get; set; } = new();
    public List<LibraryItemSearchResult> Podcast { get; set; } = new();
}