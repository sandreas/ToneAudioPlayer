using System.Net.Http;
using System.Net.Http.Headers;
using ToneAudioPlayer.Services;

namespace ToneAudioPlayer.Api.Credentials;

public class TokenCredentials: IApiCredentials
{
    public string Token { get; set; } = "";
    public string UrlString { get; set; } = "";

    public void ModifyHeaders(HttpClient client)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
    }
}