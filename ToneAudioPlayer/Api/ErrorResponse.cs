using ToneAudioPlayer.Api;

namespace ToneAudioPlayer.Services.Responses;

public class ErrorResponse : AbstractResponse
{
    public string Error { get; set; } = "";
}