# ToneAudioPlayer
Cross platform audio player inspired by the iPod Nano 7G (my favorite Audio Player)

Currently not ready for release and a work in progress. 

## Screenshots

<img src="doc/img/ToneAudioPlayer_screenshot.png" width="300" alt="Tone Audio Player" />

## Touchpad
- usage: https://wiki.archlinux.org/title/libinput
```bash
libinput list-devices
```

```
Device:           SynPS/2 Synaptics TouchPad
Kernel:           /dev/input/event4
Group:            6
Seat:             seat0, default
Size:             96x60mm
Capabilities:     pointer gesture
Tap-to-click:     disabled
Tap-and-drag:     enabled
Tap drag lock:    disabled
Left-handed:      disabled
Nat.scrolling:    disabled
Middle emulation: disabled
Calibration:      n/a
Scroll methods:   *two-finger edge 
Click methods:    *button-areas clickfinger 
Disable-w-typing: enabled
Disable-w-trackpointing: enabled
Accel profiles:   flat *adaptive custom
Rotation:         n/a
```


## Ideas
- Download
  - Id: string
  - Files: DownloadFile
    - Id: string
    - Uri: 


```c#
var ds = new DataSource(_abs);

var results = ds.Search();
var 

var dsItem = ds.GetCurrentItem();
if(dsItem != null) {
    var vm = _router.GoTo<PlayerViewModel>();
    vm.CurrentItem = dsItem;
} else {
    var vm = _router.GoTo<HomeViewModel>();
}

enum DataSourceAction {
    Play,
    Pause,
    Stop,
    NextChapter,
    PreviousChapter,
    Next,
    Previous,
    
}

ds.HandleAction(DataSourceAction.Play, vm.CurrentItem);


class Progress {
    public string MediaFileId = "";
    public TimeSpan CurrentPosition = TimeSpan.Zero;
    public TimeSpan TotalDuration = TimeSpan.Zero;
    public bool IsFinished = false;
}

class MediaFile {
    public string Id = "";
    public AudioMetadata Metadata = new();
}



enum DataSourceItemType {
    AudioBook,
    
    /*
    Playlist,
    SmartPlaylist,
    Album,
    Podcast
    */
}

interface IDataSourceItem {
    
    public Progress Progress = new();
    public PlaybackHistory History = new();
    public AudioMetadata Metadata = new();
    public List<MediaFile> MediaFiles = new();  
}

class Playlist : IDataSourceItem {
    
}

```