using AudiobookshelfApi.Models;

namespace AudiobookshelfApi.ResponseModels;

public class LibrariesResponse
{
    public List<Library> Libraries { get; set; } = new();
}