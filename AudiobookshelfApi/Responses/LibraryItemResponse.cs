using AudiobookshelfApi.Models;

namespace AudiobookshelfApi.Responses;

public class LibraryItemResponse: AbstractResponse
{
    public string Id { get; set; }
    public List<LibraryFile> LibraryFiles { get; set; } = new();
}