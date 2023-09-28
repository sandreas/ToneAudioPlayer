namespace AudiobookshelfApi.Models;

public class Media
{
    public Metadata Metadata { get; set; } = new();
    public List<AudioFile> AudioFiles { get; set; } = new();

    public MediaProgress UserMediaProgress { get; set; } = new();

    /*
   "userMediaProgress": {
       "id": "li_8gch9ve09orgn4fdz8",
       "libraryItemId": "li_8gch9ve09orgn4fdz8",
       "episodeId": null,
       "duration": 6004.6675,
       "progress": 0.002710910637433297,
       "currentTime": 16.278117,
       "isFinished": false,
       "hideFromContinueListening": false,
       "lastUpdate": 1650621052495,
       "startedAt": 1650621052495,
       "finishedAt": null
       },
     */
}