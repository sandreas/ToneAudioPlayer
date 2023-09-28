namespace AudiobookshelfApi.Models;

public class MediaProgress
{
    public string Id { get; set; } = "";
    public string LibraryItemId { get; set; } = "";
    public string EpisodeId { get; set; } = "";
    public double? Duration { get; set; }
    public double? Progress { get; set; }
    public double CurrentTime { get; set; }
    public bool IsFinished { get; set; }
    public bool HideFromContinueListening { get; set; }
    public long LastUpdate { get; set; }
    public long StartedAt { get; set; }
    public long? FinishedAt { get; set; }
}