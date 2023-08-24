namespace AudiobookshelfApi.Requests;

public class MediaProgressRequest
{
    public string? Id { get; set; }
    public string? LibraryItemId { get; set; }
    public string? EpisodeId { get; set; }
    public float? Duration { get; set; }
    public float? Progress { get; set; }
    public float? CurrentTime { get; set; }
    public bool? IsFinished { get; set; }
    public bool? HideFromContinueListening { get; set; }
    public long? LastUpdate { get; set; }
    public long? StartedAt { get; set; }
    public long? FinishedAt { get; set; }
}