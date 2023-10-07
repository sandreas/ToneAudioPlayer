using AudiobookshelfApi.Models;

namespace AudiobookshelfApi.ResponseModels;

public class LibraryItemResponse
{
    public string Id { get; set; } = "";
    public List<LibraryFile> LibraryFiles { get; set; } = new();
}