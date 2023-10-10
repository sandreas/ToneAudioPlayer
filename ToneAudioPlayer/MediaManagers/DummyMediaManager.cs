using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DynamicData;
using MediaManager;
using MediaManager.Library;
using MediaManager.Media;
using MediaManager.Notifications;
using MediaManager.Playback;
using MediaManager.Player;
using MediaManager.Queue;
using MediaManager.Volume;

namespace ToneAudioPlayer.MediaManagers;

public class DummyMediaManager : MediaManagerBase, IMediaManager
{

    public event PropertyChangedEventHandler? PropertyChanged;
    public Task Play()
    {
        return Task.FromResult(true);
    }

    public Task Pause()
    {
        return Task.FromResult(true);

    }
    
    public Task Stop()
    {
        return Task.FromResult(true);
    }

    public Task<bool> PlayPrevious()
    {
        return Task.FromResult(true);
    }

    public Task<bool> PlayNext()
    {
        return Task.FromResult(true);
    }

    public Task<bool> PlayQueueItem(IMediaItem mediaItem)
    {
        return Task.FromResult(true);
    }

    public Task<bool> PlayQueueItem(int index)
    {
        return Task.FromResult(true);
    }

    public Task StepForward()
    {
        return Task.FromResult(true);
    }

    public Task StepBackward()
    {
        return Task.FromResult(true);
    }

    public Task SeekTo(TimeSpan position)
    {
        return Task.FromResult(true);
    }

    public TimeSpan StepSize { get; set; }
    public override INotificationManager Notification { get; set; }
    public MediaPlayerState State { get; }
    public override TimeSpan Position { get; }
    public override TimeSpan Duration { get; }
    public override float Speed { get; set; }
    public override bool KeepScreenOn { get; set; }

    // public TimeSpan Position { get; }
    // public TimeSpan Duration { get; }
    public TimeSpan Buffered { get; }
    // public float Speed { get; set; }
    public RepeatMode RepeatMode { get; set; }
    public ShuffleMode ShuffleMode { get; set; }
    public bool ClearQueueOnPlay { get; set; }
    public bool AutoPlay { get; set; }
    // public bool KeepScreenOn { get; set; }
    public bool RetryPlayOnFailed { get; set; }
    public bool PlayNextOnFailed { get; set; }
    public int MaxRetryCount { get; set; }
    public event StateChangedEventHandler? StateChanged;
    public event BufferedChangedEventHandler? BufferedChanged;
    public event PositionChangedEventHandler? PositionChanged;
    public event MediaItemFinishedEventHandler? MediaItemFinished;
    public event MediaItemChangedEventHandler? MediaItemChanged;
    public event MediaItemFailedEventHandler? MediaItemFailed;
    public void Dispose()
    {
    }

    public void Init()
    {
    }

    public Task<IMediaItem> Play(IMediaItem mediaItem)
    {
        return Task.FromResult(mediaItem);
    }

    public Task<IMediaItem> Play(string uri)
    {
        return Play(new MediaItem(uri));
    }

    public Task<IMediaItem> PlayFromAssembly(string resourceName, Assembly assembly = null)
    {
        return Play("about:blank");
    }

    public Task<IMediaItem> PlayFromResource(string resourceName)
    {
        return Play("about:blank");
    }

    public Task<IMediaItem> Play(IEnumerable<IMediaItem> mediaItems)
    {
        Queue.Clear();
        Queue.AddRange(mediaItems);
        StateChanged?.Invoke(this, new StateChangedEventArgs(MediaPlayerState.Playing));
        PositionChanged?.Invoke(this, new PositionChangedEventArgs(TimeSpan.FromSeconds(1)));
        return Task.FromResult(Queue.First());
    }

    public Task<IMediaItem> Play(IEnumerable<string> items)
    {
        return Play(items.Select(i => new MediaItem(i)));
    }

    public Task<IMediaItem> Play(FileInfo file)
    {
        return Play("about:blank");
    }

    public Task<IMediaItem> Play(DirectoryInfo directoryInfo)
    {
        return Play("about:blank");
    }

    public Task<IMediaItem> Play(Stream stream, string cacheName)
    {
        return Play("about:blank");
    }

    public override IMediaPlayer MediaPlayer { get; set; }
    public override IMediaExtractor Extractor { get; set; }
    public override IVolumeManager Volume { get; set; }

    // public IMediaPlayer MediaPlayer { get; set; }
    public IMediaLibrary Library { get; set; }
    public Dictionary<string, string> RequestHeaders { get; set; }
    // public INotificationManager Notification { get; set; }
    // public IVolumeManager Volume { get; set; }
    // public IMediaExtractor Extractor { get; set; }
    public IMediaQueue Queue { get; set; } = new DummyMediaQueue();
}