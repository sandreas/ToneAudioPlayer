namespace AudiobookshelfApi.Responses;

public abstract class AbstractResponse
{
    public HttpResponseMessage Message { get; set; } = new();
}