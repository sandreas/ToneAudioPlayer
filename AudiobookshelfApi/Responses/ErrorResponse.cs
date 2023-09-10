using System.Net;
using System.Text.Json.Serialization;

namespace AudiobookshelfApi.Responses;

public class ErrorResponse: AbstractResponse
{
    [JsonIgnore]
    public Exception? Exception { get; set; } = null;

    [JsonIgnore]
    public HttpStatusCode StatusCode => Message.StatusCode;
    
    [JsonIgnore]
    public string Error => RawContent;

    
}