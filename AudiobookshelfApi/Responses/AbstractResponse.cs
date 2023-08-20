namespace AudiobookshelfApi.Responses;

public abstract class AbstractResponse
{
    public string RawContent { get; set; } = "";
    public HttpResponseMessage Message { get; set; } = new();
}