namespace AudiobookshelfApi.Models;

public class Chapter
{
    public long Id { get; set; }
    public double Start { get; set; }
    public double End { get; set; }
    public string Title { get; set; } = "";
}