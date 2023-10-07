using System.Net;
using System.Text.Json.Serialization;

namespace AudiobookshelfApi.Responses;

public class ErrorResponse: Response<Error>
{
    [JsonIgnore]
    public Exception? Exception { get; set; }

    [JsonIgnore]
    public HttpStatusCode StatusCode => Message?.StatusCode ?? HttpStatusCode.NotFound;
    
    [JsonIgnore]
    public string Error => RawContent;

    public ErrorResponse(HttpResponseMessage responseMessage) : base(new Error(), responseMessage)
    {
    }

}