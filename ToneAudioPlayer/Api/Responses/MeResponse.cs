using System.Collections.Generic;

namespace ToneAudioPlayer.Api.Responses;

public class MeResponse : AbstractResponse
{
    public string Id { get; set; } = "";
    public string Username { get; set; } = "";
    public string Type { get; set; } = "";

    public string Token { get; set; } = "";

    // public List<object> mediaProgress { get; set; }
    // public List<object> seriesHideFromContinueListening { get; set; }
    // public List<object> bookmarks { get; set; }
    public bool IsActive { get; set; }
    public bool IsLocked { get; set; }
    public long LastSeen { get; set; }
    public long CreatedAt { get; set; }

    public Dictionary<string, bool> Permissions { get; set; } = new();
    // public List<object> librariesAccessible { get; set; }
    // public List<object> itemTagsAccessible { get; set; }
}