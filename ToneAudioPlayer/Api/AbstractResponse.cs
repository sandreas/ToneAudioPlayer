using System.Net.Http;

namespace ToneAudioPlayer.Api;

public abstract class AbstractResponse: IResponse
{
    public HttpResponseMessage Message { get; set; } = new();
}