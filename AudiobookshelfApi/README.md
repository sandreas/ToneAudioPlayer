# Audiobookshelf API

## What is this?

`AudiobookshelfApi` is an SDK for interacting with the the awesome [audiobookshelf] Self-hosted audiobook and podcast server [API].

## State

Currently, this is a pretty early state of development and does only support a minimum of the provided API. However, there is hopefully more to come.

## Example

```c#



var credentials = new Credentials()
{
    BaseAddress = new Uri(Environment.GetEnvironmentVariable("AUDIOBOOKSHELF_API_URL")??""),
    Username=Environment.GetEnvironmentVariable("AUDIOBOOKSHELF_USERNAME") ?? "root",
    Password=Environment.GetEnvironmentVariable("AUDIOBOOKSHELF_PASSWORD") ?? "",
};
var http = new HttpClient();
var api = new Audiobookshelf(http, credentials);

// login
var loginResponse = await api.LoginAsync(credentials);
if (loginResponse is not LoginResponse lr)
{
    HandleError(loginResponse, "Login");
    return;
}
Console.WriteLine($"Login successful: {lr.User.Id}" );

// get libraries
var librariesResponse = await api.GetLibrariesAsync();
if (librariesResponse is not LibrariesResponse libr)
{
    HandleError(loginResponse, "Libraries");
    return;
}

if (!libr.Libraries.Any())
{
    Console.WriteLine("No libraries found");
    return;
}

Console.WriteLine($"Found libraries");
foreach (var lib in libr.Libraries)
{
    Console.WriteLine($"  {lib.Name}");
}

var selectedLib = libr.Libraries.First();

// perform a search
var searchResponse = await api.SearchLibraryAsync(selectedLib.Id, "Harry Potter");
if (searchResponse is not SearchLibraryResponse sr)
{
    HandleError(searchResponse, "Search");
    return;
}

if (!sr.Book.Any() && !sr.Podcast.Any())
{
    Console.WriteLine("No items found");
    return;
}

Console.WriteLine("Search matches:");
foreach (var book in sr.Book)
{
    Console.WriteLine($"  {book.MatchKey}={book.MatchText}: {book.LibraryItem.Media.Metadata.Title}");

    foreach (var audioFile in book.LibraryItem.Media.AudioFiles)
    {
        var url = $"{http.BaseAddress?.ToString().TrimEnd('/')}/api/items/{book.LibraryItem.Id}/file/{audioFile.Ino}?token={lr.User.Token}";
        Console.WriteLine($"    Playback URI: {url}");
    }

}



void HandleError(AbstractResponse response, string prefix)
{
    if(response is ErrorResponse e)
    {
        Console.WriteLine($"{prefix}: {e.Error} ({e.StatusCode})");
    }
    else
    {
        Console.WriteLine($"{prefix}: unknown error"); 
    }
}
```


[audiobookshelf]: https://www.audiobookshelf.org/
[API]: https://api.audiobookshelf.org/