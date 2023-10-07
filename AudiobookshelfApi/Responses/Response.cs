namespace AudiobookshelfApi.Responses;

public class Response<T> : IResponse where T:class
{
    public T Value { get; set; }
    public HttpResponseMessage? Message { get; set;  }
    public string RawContent { get; set; } = "";

    public Response(T value, HttpResponseMessage? message=null)
    {
        Value = value;
        Message = message;
    }
}