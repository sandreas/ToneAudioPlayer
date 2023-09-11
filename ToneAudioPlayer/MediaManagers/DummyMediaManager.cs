using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using MediaManager;
using MediaManager.Library;
using MediaManager.Media;
using MediaManager.Notifications;
using MediaManager.Playback;
using MediaManager.Player;
using MediaManager.Queue;
using MediaManager.Volume;

namespace ToneAudioPlayer.MediaManagers;

public class DummyMediaManager : IMediaManager
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public Task Play()
    {
        throw new NotImplementedException();
    }

    public Task Pause()
    {
        throw new NotImplementedException();
    }

    
    public Task Stop()
    {
        throw new NotImplementedException();
    }

    public Task<bool> PlayPrevious()
    {
        throw new NotImplementedException();
    }

    public Task<bool> PlayNext()
    {
        throw new NotImplementedException();
    }

    public Task<bool> PlayQueueItem(IMediaItem mediaItem)
    {
        throw new NotImplementedException();
    }

    public Task<bool> PlayQueueItem(int index)
    {
        throw new NotImplementedException();
    }

    public Task StepForward()
    {
        throw new NotImplementedException();
    }

    public Task StepBackward()
    {
        throw new NotImplementedException();
    }

    public Task SeekTo(TimeSpan position)
    {
        throw new NotImplementedException();
    }

    public TimeSpan StepSize { get; set; }
    public MediaPlayerState State { get; }
    public TimeSpan Position { get; }
    public TimeSpan Duration { get; }
    public TimeSpan Buffered { get; }
    public float Speed { get; set; }
    public RepeatMode RepeatMode { get; set; }
    public ShuffleMode ShuffleMode { get; set; }
    public bool ClearQueueOnPlay { get; set; }
    public bool AutoPlay { get; set; }
    public bool KeepScreenOn { get; set; }
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
        throw new NotImplementedException();
    }

    public void Init()
    {
        throw new NotImplementedException();
    }

    public Task<IMediaItem> Play(IMediaItem mediaItem)
    {
        throw new NotImplementedException();
    }

    public Task<IMediaItem> Play(string uri)
    {
        throw new NotImplementedException();
    }

    public Task<IMediaItem> PlayFromAssembly(string resourceName, Assembly assembly = null)
    {
        throw new NotImplementedException();
    }

    public Task<IMediaItem> PlayFromResource(string resourceName)
    {
        throw new NotImplementedException();
    }

    public Task<IMediaItem> Play(IEnumerable<IMediaItem> mediaItems)
    {
        throw new NotImplementedException();
    }

    public Task<IMediaItem> Play(IEnumerable<string> items)
    {
        throw new NotImplementedException();
    }

    public Task<IMediaItem> Play(FileInfo file)
    {
        throw new NotImplementedException();
    }

    public Task<IMediaItem> Play(DirectoryInfo directoryInfo)
    {
        throw new NotImplementedException();
    }

    public Task<IMediaItem> Play(Stream stream, string cacheName)
    {
        throw new NotImplementedException();
    }

    public IMediaPlayer MediaPlayer { get; set; }
    public IMediaLibrary Library { get; set; }
    public Dictionary<string, string> RequestHeaders { get; set; }
    public INotificationManager Notification { get; set; }
    public IVolumeManager Volume { get; set; }
    public IMediaExtractor Extractor { get; set; }
    public IMediaQueue Queue { get; set; }
}