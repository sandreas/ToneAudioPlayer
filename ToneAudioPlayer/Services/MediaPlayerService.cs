
using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls.Primitives;
using DynamicData;
using MediaManager;
using MediaManager.Playback;
using ToneAudioPlayer.DataSources;
using ToneAudioPlayer.DataSources.Audiobookshelf;
using MediaItem = MediaManager.Library.MediaItem;

namespace ToneAudioPlayer.Services;



public class MediaPlayerService
{
    
    private readonly IMediaManager _mediaManager;
    private readonly AudiobookshelfDataSource _dataSource;

    public event StateChangedEventHandler? StateChanged;
    public event PositionChangedEventHandler? PositionChanged;

    
    private DataSourceItem? _mediaItem;
    
    public MediaPlayerService(AudiobookshelfDataSource dataSource, IMediaManager mediaManager)
    {

        _dataSource = dataSource;
        _mediaManager = mediaManager;
        _mediaManager.StateChanged += OnStateChanged;
        _mediaManager.PositionChanged += OnPositionChanged;

        /*
        _mediaManager.BufferedChanged;
        _mediaManager.PositionChanged;
        _mediaManager.PropertyChanged;
        _mediaManager.MediaItemChanged;
        _mediaManager.MediaItemFailed;
        _mediaManager.MediaItemFinished;
        */

        // _mediaManager.StateChanged += 
        // _mediaManager.PositionChanged += OnPositionChanged;




        /*
        _mediaPlayer.TimeChanged += TimeChanged;
        _mediaPlayer.LengthChanged += LengthChanged;
        _mediaPlayer.EndReached += EndReached;
        _mediaPlayer.Playing += Playing;
        _mediaPlayer.Paused += Paused;
        */
    }

    private void OnPositionChanged(object sender, PositionChangedEventArgs e)
    {
        PositionChanged?.Invoke(sender, e);
    }

    private void OnStateChanged(object sender, StateChangedEventArgs e)
    {
        StateChanged?.Invoke(sender, e);
        /*
        e.State 
        public enum MediaPlayerState
        {
            Stopped,
            Loading,
            Buffering,
            Playing,
            Paused,
            Failed
        }
        */
    }

    
    
    /*
    public async Task ResumeMedia(DataSourceItem item)
    {
        // Todo:
        // https://api.audiobookshelf.org/#get-a-media-progress
        // var currentTime = await _dataSource.GetMediaProgressAsync(item);

        await UpdateMediaAsync(item);
        await SeekTo(currentTime);
    }
    */
    
    public async Task UpdateItemAsync(DataSourceItem item)
    {
        if (_mediaItem == item)
        {
            return;
        }
        
        if (_mediaItem != null)
        {
            _dataSource.HandleAction(_mediaItem, DataSourceAction.Remove);
        }

        _mediaItem = item;
        _dataSource.HandleAction(item, DataSourceAction.Insert);


        var activeMedia = item.MediaItems.Count >= item.MediaItemIndex
            ? item.MediaItems.ElementAtOrDefault(item.MediaItemIndex)
            : item.MediaItems.FirstOrDefault();
        if (activeMedia == null)
        {
            return;
        }
            
        var mediaItems = item.MediaItems.Select(u => new MediaItem(u.Url.ToString())).ToList();
        
        /*
         // todo: check if this is the better way
        _mediaManager.Queue.Clear();
        _mediaManager.Queue.AddRange(mediaItems);
        _mediaManager.Queue.CurrentIndex = item.MediaItemIndex;
        await _mediaManager.SeekTo(activeMedia.Position);
        */
        
        // don't use local Play/Pause/SeekTo to prevent HandleEvent on DataSource
        await _mediaManager.Play(mediaItems);
        await _mediaManager.Pause();
        _mediaManager.Queue.CurrentIndex = item.MediaItemIndex;
        await _mediaManager.SeekTo(activeMedia.Position);
    }

    public async Task ToggleAsync()
    {
        if (IsPlaying)
        {
            await PauseAsync();
        }
        else
        {
            await PlayAsync();
        }
    }

    public TimeSpan Duration => _mediaManager.Duration;
    
    public bool IsPlaying => _mediaItem?.Status == DataSourceItemStatus.Playing || _mediaManager.IsPlaying();


    public async Task PlayAsync()
    {
        _dataSource.HandleAction(_mediaItem, DataSourceAction.Play);
        await _mediaManager.Play(); 
    }

    public async Task PauseAsync() 
    {
        _dataSource.HandleAction(_mediaItem, DataSourceAction.Pause);
        await _mediaManager.Pause(); 
    }

    public async Task SeekTo(TimeSpan t)    {
        _dataSource.HandleAction(_mediaItem, DataSourceAction.SeekTo, t);
        await _mediaManager.SeekTo(t); 
    }
    public Task Seek(TimeSpan offset) => SeekTo(_mediaManager.Position + offset);

    
    public Task NextChapter() => Seek(TimeSpan.FromMinutes(5));
    public Task PreviousChapter() =>  Seek(TimeSpan.FromMinutes(-5));
    
    
    /*
    private async Task<bool> StoreProgressAsync(TimeSpan position)
    {
        if (_mediaItem == null)
        {
            return false;
        }
        return await _dataSource.UpdateProgressAsync(_mediaItem, position, _mediaManager.Duration);
    }
    */
}



/*
    public class PlaybackService
    {
        readonly LibVLC _libVLC;
        readonly MediaPlayer _mp;
        const long OFFSET = 5000;

        public PlaybackService()
        {
            if (DesignMode.IsDesignModeEnabled) return;

            Core.Initialize();

            _libVLC = new LibVLC();

            _mp = new MediaPlayer(_libVLC);
        }

        public void Init()
        {
            // create a libvlc media
            // disable video output, we only need audio
            using (var media = new Media(_libVLC, new Uri("https://archive.org/download/ImagineDragons_201410/imagine%20dragons.mp4"), ":no-video"))
                _mp.Media = media;

            // subscribe to libvlc playback events
            _mp.TimeChanged += TimeChanged;
            _mp.PositionChanged += PositionChanged;
            _mp.LengthChanged += LengthChanged;
            _mp.EndReached += EndReached;
            _mp.Playing += Playing;
            _mp.Paused += Paused;

            // subscribe to UI app events for seeking.

            MessagingCenter.Subscribe<string>(MessengerKeys.App, MessengerKeys.Rewind, vm =>
            {
                Debug.WriteLine("Rewind");
                _mp.Time -= OFFSET;
            });

            MessagingCenter.Subscribe<string>(MessengerKeys.App, MessengerKeys.Forward, vm =>
            {
                Debug.WriteLine("Forward");
                _mp.Time += OFFSET;
            });
        }


        public void Play(bool play)
        {
            if (play)
                _mp.Play();
            else _mp.Pause();
        }

        // when the libvlc mediaplayer events fire, publish an event with the MessagingCenter

        private void PositionChanged(object sender, MediaPlayerPositionChangedEventArgs e) =>
            MessagingCenter.Send(MessengerKeys.App, MessengerKeys.Position, e.Position);

        private void Paused(object sender, System.EventArgs e) =>
            MessagingCenter.Send(MessengerKeys.App, MessengerKeys.Play, false);

        private void Playing(object sender, System.EventArgs e) =>
            MessagingCenter.Send(MessengerKeys.App, MessengerKeys.Play, true);

        private void EndReached(object sender, System.EventArgs e) =>
            MessagingCenter.Send(MessengerKeys.App, MessengerKeys.EndReached);

        private void LengthChanged(object sender, MediaPlayerLengthChangedEventArgs e) =>
            MessagingCenter.Send(MessengerKeys.App, MessengerKeys.Length, e.Length);

        private void TimeChanged(object sender, MediaPlayerTimeChangedEventArgs e) =>
            MessagingCenter.Send(MessengerKeys.App, MessengerKeys.Time, e.Time);
    }
    */