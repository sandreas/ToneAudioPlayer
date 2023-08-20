using AudiobookshelfApi.Models;

namespace AudiobookshelfApi.Responses;

public class LibraryItemsResponse: AbstractResponse
{
    public List<LibraryItem> Results { get; set; } = new();
}