using System.Net.Http;

namespace ToneAudioPlayer.Api;

public interface IResponse
{
    public HttpResponseMessage Message { get; }
}