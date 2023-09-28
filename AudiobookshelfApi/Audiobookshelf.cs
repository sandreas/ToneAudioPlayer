using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AudiobookshelfApi.Api;
using AudiobookshelfApi.Models;
using AudiobookshelfApi.Requests;
using AudiobookshelfApi.Responses;
using Avalonia.Media.Imaging;
using ToneAudioPlayer.Api;

namespace AudiobookshelfApi;

public class Audiobookshelf
{
    
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
    
    private readonly HttpClient _http;
    private readonly Credentials _credentials = new();

    public bool IsAuthenticated => VerifyToken(_credentials.Token, TimeSpan.Zero);
    public bool IsConfigured => !string.IsNullOrEmpty(_http.BaseAddress?.ToString());
    
    
    public Audiobookshelf(HttpClient http, Credentials credentials)
    {
        _http = http;
        UpdateCredentials(credentials);
    }
    
    public async Task<AbstractResponse> LoginAsync(Credentials? credentials=null, CancellationToken? cancellationToken = null)
    {
        if (credentials != null)
        {
            UpdateCredentials(credentials);            
        }
        return await ValidateOrRenewTokenAsync(TimeSpan.Zero, cancellationToken);
    }
    
    private void UpdateCredentials(Credentials credentials)
    {
        _credentials.Username = credentials.Username;
        _credentials.Password = credentials.Password;
        _credentials.Token = credentials.Token;
        _http.BaseAddress = credentials.BaseAddress;

    }
    
    private async Task<AbstractResponse> ValidateOrRenewTokenAsync(TimeSpan? refreshTimeBuffer = null, CancellationToken? cancellationToken = null)
    {
        if (VerifyToken(_credentials.Token, refreshTimeBuffer ?? TimeSpan.Zero))
        {
            ApplyAuthorizationHeader();
            return new LoginResponse()
            {
                User = new User()
                {
                    Token = _credentials.Token
                }
            };
        }

        var loginRequest = new LoginRequest
        {
            Username = _credentials.Username,
            Password = _credentials.Password,
        };

        var responseMessage = await PostJsonAsync("login", loginRequest, cancellationToken ?? CancellationToken.None);
        
        if (!responseMessage.IsSuccessStatusCode)
        {
            return await GenerateResponseAsync<ErrorResponse>(responseMessage);
        }

        var successResponse = await GenerateResponseAsync<LoginResponse>(responseMessage);
        if (successResponse is not LoginResponse loginResponse)
        {
            return await GenerateResponseAsync<ErrorResponse>(responseMessage);
        }

        _credentials.Token = loginResponse.User.Token;
        ApplyAuthorizationHeader();
        return loginResponse;
    }
    
    private static async Task<AbstractResponse> GenerateResponseAsync<T>(HttpResponseMessage responseMessage) where T : AbstractResponse, new()
    {
        try
        {
            var response = responseMessage.IsSuccessStatusCode
                ? await responseMessage.Content.ReadFromJsonAsync<T>() ?? new T()
                {
                    RawContent = await responseMessage.Content.ReadAsStringAsync()
                }
                : new T
                {
                    RawContent = await responseMessage.Content.ReadAsStringAsync()
                };
            response.Message = responseMessage;
            return response;
        }
        catch (Exception e)
        {
            var r = new ErrorResponse()
            {
                Exception = e,
                RawContent = await responseMessage.Content.ReadAsStringAsync()
            };
            return r;
        }

    }

    private async Task<HttpResponseMessage> PostJsonAsync<T>(string url, T request, CancellationToken ct)
    {
        // todo: use SerializeAsync(new MemoryStream(), request, options)
        var json = JsonSerializer.Serialize(request, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        return await _http.PostAsync(url, content, ct);
    }
    private async Task<HttpResponseMessage> PatchJsonAsync<T>(string url, T request, CancellationToken ct)
    {
        // todo: use SerializeAsync(new MemoryStream(), request, options)
        var json = JsonSerializer.Serialize(request, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        return await _http.PatchAsync(url, content, ct);
    }

    private static bool VerifyToken(string token, TimeSpan rt)
    {
        try
        {
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);
            return DateTime.UtcNow < jwtSecurityToken.ValidFrom || jwtSecurityToken.ValidTo < DateTime.UtcNow + rt;
        }
        catch (Exception)
        {
            return false;
        }
    }
    
    private void ApplyAuthorizationHeader()
    {
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _credentials.Token);
    }
    
    public async Task<AbstractResponse> GetLibrariesAsync(CancellationToken? cancellationToken = null)
    {
        await ValidateOrRenewTokenAsync();
        var responseMessage = await _http.GetAsync("api/libraries", cancellationToken ?? CancellationToken.None);
        if (responseMessage.IsSuccessStatusCode)
        {
            return await GenerateResponseAsync<LibrariesResponse>(responseMessage);
        }
        return await GenerateResponseAsync<ErrorResponse>(responseMessage);
    }
    
    public async Task<AbstractResponse> SearchLibraryAsync(string libraryId, string query, CancellationToken? cancellationToken = null)
    {
        await ValidateOrRenewTokenAsync();

        // GET https://abs.example.com/api/libraries/<ID>/search?<q>
        var responseMessage = await _http.GetAsync(new Uri($"{_http.BaseAddress?.ToString().TrimEnd('/')}/api/libraries/{libraryId}/search?q={query}&limit=12"), cancellationToken ?? CancellationToken.None);
        // for debugging
        // var content = await responseMessage.Content.ReadAsStringAsync();
        if (responseMessage.IsSuccessStatusCode)
        {
            return await GenerateResponseAsync<SearchLibraryResponse>(responseMessage);
        }
        return await GenerateResponseAsync<ErrorResponse>(responseMessage);
    }
    
    public async Task<AbstractResponse> GetLibraryItemsAsync(string libraryId, CancellationToken? cancellationToken = null)
    {
        await ValidateOrRenewTokenAsync();

        var responseMessage = await _http.GetAsync($"api/libraries/{libraryId}/items?limit=5", cancellationToken ?? CancellationToken.None);
        if (responseMessage.IsSuccessStatusCode)
        {
            return await GenerateResponseAsync<LibraryItemsResponse>(responseMessage);
        }
        return await GenerateResponseAsync<ErrorResponse>(responseMessage);
    }
    
    public async Task<AbstractResponse> GetLibraryItem(string itemId, CancellationToken? cancellationToken = null)
    {
        await ValidateOrRenewTokenAsync();

        var responseMessage = await _http.GetAsync($"api/items/{itemId}", cancellationToken ?? CancellationToken.None);
        if (responseMessage.IsSuccessStatusCode)
        {
            // todo:
            // - Decouple Model from Response (SuccessResponse<LibraryItem>, ErrorResponse<Error>)
            // 
            return await GenerateResponseAsync<LibraryItem>(responseMessage);
        }
        return await GenerateResponseAsync<ErrorResponse>(responseMessage);
    }
    
    public async Task<AbstractResponse> GetMediaProgressAsync(string libraryItemId, string? episodeId = null, CancellationToken? cancellationToken = null)
    {
        await ValidateOrRenewTokenAsync();
        // me/progress/

        var responseMessage = await _http.GetAsync(BuildProgressEndpoint(libraryItemId, episodeId), cancellationToken ?? CancellationToken.None);
        if (responseMessage.IsSuccessStatusCode)
        {
            return await GenerateResponseAsync<MediaProgressResponse>(responseMessage);
        }
        return await GenerateResponseAsync<ErrorResponse>(responseMessage);
    }

    private static string BuildProgressEndpoint(string libraryItemId, string? episodeId = null)
    {
        var endpoint = $"api/me/progress/{libraryItemId}";
        if (!string.IsNullOrEmpty(episodeId))
        {
            endpoint += $"/{episodeId}";
        }

        return endpoint;
    }
    
    public async Task<AbstractResponse> UpdateMediaProgressAsync(MediaProgressRequest mediaProgress, string libraryItemId, string? episodeId = null,  CancellationToken? cancellationToken = null)
    {
        await ValidateOrRenewTokenAsync();
        // me/progress/
        
        var responseMessage = await PatchJsonAsync(BuildProgressEndpoint(libraryItemId, episodeId), mediaProgress, cancellationToken ?? CancellationToken.None);
        try
        {
            
            if (responseMessage.IsSuccessStatusCode)
            {
                // response is NON json but just `OK`, so manually generate a response
                return new MediaProgressResponse()
                {
                    Progress = mediaProgress.Progress ?? 0,
                    IsFinished = mediaProgress.IsFinished ?? false
                };
                /*
                return await GenerateResponseAsync<MediaProgressResponse>(responseMessage);
                */
            }
            return await GenerateResponseAsync<ErrorResponse>(responseMessage);
        }
        catch (Exception e)
        {
            // var content = await responseMessage.Content.ReadAsStringAsync();
            
            return new ErrorResponse()
            {
                RawContent = e.Message
            };
        }


    }


    public Uri BuildMediaUrl(string id, string ino/*LibraryItem item*/)
    {
        return new Uri($"{_http.BaseAddress?.ToString().TrimEnd('/')}/api/items/{id}/file/{ino}?token={_credentials.Token}");
        /*
        return item.Media.AudioFiles.Select(f =>
                new DataSourceMediaItem()
                {
                    Id = f.Ino.ToString(),
                    Url = new Uri($"{_http.BaseAddress?.ToString().TrimEnd('/')}/api/items/{item.Id}/file/{f.Ino}?token={_credentials.Token}"),
                    FileName = f.Metadata.Filename,
                    Extension = f.Metadata.Ext, // including .
                    // OriginalPath = f.Metadata.Path
                })
            .ToArray();
        */
    }
    
    
    public async Task<MemoryStream?> LoadLibraryItemCover(string id)
    {
        var url = new Uri($"{_http.BaseAddress?.ToString().TrimEnd('/')}/api/items/{id}/cover");
        // var url = new Uri("https://placehold.co/400x400.jpg?text=test");
        try
        {
            var response = await _http.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsByteArrayAsync();
            return new MemoryStream(data);
        }
        catch (HttpRequestException)
        {
            // Console.WriteLine($"An error occurred while downloading image '{url}' : {ex.Message}");
            return null;
        }
    }
    
}