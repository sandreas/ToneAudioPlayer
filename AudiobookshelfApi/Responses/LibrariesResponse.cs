using AudiobookshelfApi.Models;

namespace AudiobookshelfApi.Responses;

public class LibrariesResponse: AbstractResponse
{
    public List<Library> Libraries { get; set; } = new();
}