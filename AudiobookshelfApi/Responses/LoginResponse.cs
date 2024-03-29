using AudiobookshelfApi.Models;
using AudiobookshelfApi.Responses;

namespace ToneAudioPlayer.Api;

public class LoginResponse: AbstractResponse
{
  public User User { get; set; } = new();
}

/*
{
  "user": {
    "id": "root",
    "username": "root",
    "type": "root",
    "token": "exJhbGciOiJI6IkpXVCJ9.eyJ1c2Vyi5NDEyODc4fQ.ZraBFohS4Tg39NszY",
    "mediaProgress": [
      {
        "id": "li_bufnnmp4y5o2gbbxfm-ep_lh6ko39pumnrma3dhv",
        "libraryItemId": "li_bufnnmp4y5o2gbbxfm",
        "episodeId": "ep_lh6ko39pumnrma3dhv",
        "duration": 1454.18449,
        "progress": 0.434998929881311,
        "currentTime": 632.568697,
        "isFinished": false,
        "hideFromContinueListening": false,
        "lastUpdate": 1668586015691,
        "startedAt": 1668120083771,
        "finishedAt": null
      }
    ],
    "seriesHideFromContinueListening": [],
    "bookmarks": [],
    "isActive": true,
    "isLocked": false,
    "lastSeen": 1669010786013,
    "createdAt": 1666543632566,
    "permissions": {
      "download": true,
      "update": true,
      "delete": true,
      "upload": true,
      "accessAllLibraries": true,
      "accessAllTags": true,
      "accessExplicitContent": true
    },
    "librariesAccessible": [],
    "itemTagsAccessible": []
  },
  "userDefaultLibraryId": "lib_c1u6t4p45c35rf0nzd",
  "serverSettings": {
    "id": "server-settings",
    "scannerFindCovers": false,
    "scannerCoverProvider": "audible",
    "scannerParseSubtitle": false,
    "scannerPreferAudioMetadata": false,
    "scannerPreferOpfMetadata": false,
    "scannerPreferMatchedMetadata": false,
    "scannerDisableWatcher": true,
    "scannerPreferOverdriveMediaMarker": false,
    "scannerUseSingleThreadedProber": true,
    "scannerMaxThreads": 0,
    "scannerUseTone": false,
    "storeCoverWithItem": false,
    "storeMetadataWithItem": false,
    "metadataFileFormat": "json",
    "rateLimitLoginRequests": 10,
    "rateLimitLoginWindow": 600000,
    "backupSchedule": "30 1 * * *",
    "backupsToKeep": 2,
    "maxBackupSize": 1,
    "backupMetadataCovers": true,
    "loggerDailyLogsToKeep": 7,
    "loggerScannerLogsToKeep": 2,
    "homeBookshelfView": 1,
    "bookshelfView": 1,
    "sortingIgnorePrefix": false,
    "sortingPrefixes": [
      "the",
      "a"
    ],
    "chromecastEnabled": false,
    "dateFormat": "MM/dd/yyyy",
    "language": "en-us",
    "logLevel": 2,
    "version": "2.2.5"
  },
  "Source": "docker"
}
*/