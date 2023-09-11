
using System;
using System.Linq;
using System.Threading.Tasks;
using DynamicData;
using MediaManager;
using MediaManager.Library;
using MediaManager.Playback;
using ToneAudioPlayer.DataSources;

namespace ToneAudioPlayer.Services;



public class MediaPlayerService
{
    
    private readonly IMediaManager _mediaManager;
    private readonly AudiobookshelfDataSource _dataSource;

    public event StateChangedEventHandler? StateChanged;
    public event PositionChangedEventHandler? PositionChanged;

    
    private IItemIdentifier? _mediaId;
    
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

    public async Task ResumeMedia(IItemIdentifier identifier)
    {
        // Todo:
        // https://api.audiobookshelf.org/#get-a-media-progress
        var currentTime = await _dataSource.GetMediaProgressAsync(identifier);

        await UpdateMediaAsync(identifier);
        await Play();
        await SeekTo(currentTime);
    }
    
    public async Task UpdateMediaAsync(IItemIdentifier id)
    {
        if (_mediaId != null)
        {
            await StoreProgressAsync(_mediaManager?
                .Position ?? TimeSpan.Zero);  // _dataSource.UpdateProgressAsync(_mediaId, _mediaManager.Position, _mediaManager.Duration);
        }
        
        _mediaId = id;

        if (_mediaManager != null)
        {
            await _mediaManager.Play(id.MediaUrls.Select(u => new MediaItem(u)));
            await Pause();
        }
        /*
        _mediaManager.Queue.Clear();
        _mediaManager.Queue.AddRange(id.MediaUrls.Select(u => new MediaItem(u)));
        */
    }

    public TimeSpan Duration => _mediaManager.Duration;
    
    public bool IsPlaying => _mediaManager.IsPlaying();
    public Task Play() => _mediaManager.Play(); 
    public Task Pause() =>  _mediaManager.Pause();
    public Task SeekTo(TimeSpan t) => _mediaManager.SeekTo(t);
    public Task Seek(TimeSpan offset) => SeekTo(_mediaManager.Position + offset);

    
    public Task NextChapter() => Seek(TimeSpan.FromMinutes(5));
    public Task PreviousChapter() =>  Seek(TimeSpan.FromMinutes(-5));
    
    
    private async Task<bool> StoreProgressAsync(TimeSpan position)
    {
        if (_mediaId == null)
        {
            return false;
        }
        return await _dataSource.UpdateProgressAsync(_mediaId, position, _mediaManager.Duration);
    }
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