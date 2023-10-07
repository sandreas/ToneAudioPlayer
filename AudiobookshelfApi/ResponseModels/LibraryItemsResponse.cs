using AudiobookshelfApi.Models;

namespace AudiobookshelfApi.ResponseModels;

public class LibraryItemsResponse
{
    public List<LibraryItem> Results { get; set; } = new();
}