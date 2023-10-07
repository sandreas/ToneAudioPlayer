namespace AudiobookshelfApi.Responses;

public interface IResponse
{
    // public T? Value { get; }
    public string RawContent { get; }
    public HttpResponseMessage Message { get; }
}