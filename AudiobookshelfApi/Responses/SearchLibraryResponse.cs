using AudiobookshelfApi.Models;

namespace AudiobookshelfApi.Responses;

public class SearchLibraryResponse: AbstractResponse
{
    public List<LibraryItemSearchResult> Book { get; set; } = new();
    public List<LibraryItemSearchResult> Podcast { get; set; } = new();
}