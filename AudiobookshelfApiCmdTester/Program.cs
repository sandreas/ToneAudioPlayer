using System.Text.Json;
using AudiobookshelfApi;
using AudiobookshelfApi.Api;
using AudiobookshelfApi.ResponseModels;
using AudiobookshelfApi.Responses;

// trailing slash is important

var p = "/home/andreas/projects/ToneAudioPlayer/AudiobookshelfApiCmdTester/samples/searchRepsonse.json";
var jsonString = File.ReadAllText(p);


var options = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true
};
var r = JsonSerializer.Deserialize<SearchLibraryResponse>(jsonString, options);



var testUrl = "https://kazeng.fynder.de";
var handler = new HttpClientHandler()
{
    AllowAutoRedirect = false
};
var client = new HttpClient(handler);
client.BaseAddress = new Uri(testUrl);
client.DefaultRequestHeaders.Clear();
client.DefaultRequestHeaders.ConnectionClose = true;
// wrong password delivers same response...?
var byteArray = "hampelmann:Technisat123!"u8.ToArray();
// var str = "aGFtcGVsbWFubjpUZWNobmlzYXQxMjMh";
var str = Convert.ToBase64String(byteArray);
client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", str);

// var response = await client.GetAsync("/git/");
// var response = await client.GetAsync("/audiobooks/");
var response = await client.GetAsync("/audiobooks/api/libraries");


var content = response.Content;

Console.WriteLine(response.RequestMessage?.RequestUri);
/*
{
    Console.WriteLine($"Request was redirected to {response.RequestMessage.RequestUri}");
}
*/

// ... Check Status Code                                
Console.WriteLine("Response StatusCode: " + (int)response.StatusCode);

// ... Read the string.
string result = await content.ReadAsStringAsync();

// ... Display the result.
if (result != null &&
    result.Length >= 50)
{
    Console.WriteLine(result.Substring(0, 250) + "...");
}

return;


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



void HandleError(Response<> response, string prefix)
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