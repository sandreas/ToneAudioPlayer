using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AudiobookshelfApi.Api;
using AudiobookshelfApi.Models;
using AudiobookshelfApi.Requests;
using AudiobookshelfApi.Responses;
using ToneAudioPlayer.Api;

namespace AudiobookshelfApi;

public class AudiobookshelfApi
{
    private readonly HttpClient _http;
    private readonly Credentials _credentials = new();
    public bool IsAuthenticated => VerifyToken(_credentials.Token, TimeSpan.Zero);
    
    public AudiobookshelfApi(HttpClient http, Credentials credentials)
    {
        _http = http;
        UpdateCredentials(credentials);
    }
    
    public async Task<AbstractResponse> AuthenticateAsync(Credentials credentials, CancellationToken? cancellationToken = null)
    {
        UpdateCredentials(credentials);
        return await RenewAuthenticationTokenAsync(TimeSpan.Zero, cancellationToken);
    }
    
    private void UpdateCredentials(Credentials credentials)
    {
        _credentials.Username = credentials.Username;
        _credentials.Password = credentials.Password;
        _credentials.Token = credentials.Token;
        _http.BaseAddress = credentials.BaseAddress;
    }
    
    private async Task<AbstractResponse> RenewAuthenticationTokenAsync(TimeSpan? refreshTimeBuffer = null, CancellationToken? cancellationToken = null)
    {
        var ct = cancellationToken ?? CancellationToken.None;
        var rt = refreshTimeBuffer ?? TimeSpan.Zero;

        if (VerifyToken(_credentials.Token, rt))
        {
            AddAuthorizationHeader();
            return new LoginResponse()
            {
                User = new User()
                {
                    Token = _credentials.Token
                }
            };
        }

        var loginRequest = new LoginRequest()
        {
            Username = _credentials.Username,
            Password = _credentials.Password,
        };

        var responseMessage = await PostJsonAsync("login", loginRequest, ct);
        /*
        if (!responseMessage.IsSuccessStatusCode)
        {
            return await GenerateResponseAsync<ErrorResponse>(responseMessage);
        }

        var successResponse = await GenerateResponseAsync<TokenResponse>(responseMessage);
        if (successResponse is not TokenResponse tokenResponse)
        {
            return await GenerateResponseAsync<ErrorResponse>(responseMessage);
        }

        _credentials.Token = tokenResponse.Tokens.FirstOrDefault() ?? "";
        AddAuthorizationHeader();
        return tokenResponse;
        */

        // todo: fill with life
        return new LoginResponse();
    }

    private async Task<HttpResponseMessage> PostJsonAsync<T>(string url, T request, CancellationToken ct)
    {
        
        var options = new JsonSerializerOptions()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull | JsonIgnoreCondition.WhenWritingDefault
        };
        // todo: use SerializeAsync(new MemoryStream(), request, options)
        var json = JsonSerializer.Serialize(request, options);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        return await _http.PostAsync(url, content, ct);
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
    
    private void AddAuthorizationHeader()
    {
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _credentials.Token);
    }
}