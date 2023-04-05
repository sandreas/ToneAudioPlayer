using System.Net.Http;

namespace ToneAudioPlayer.Api;

public interface IApiCredentials
{
    public string UrlString { get; }
    public void ModifyHeaders(HttpClient client);
}