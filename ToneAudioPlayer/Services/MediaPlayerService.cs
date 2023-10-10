
using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls.Primitives;
using DynamicData;
using MediaManager;
using MediaManager.Media;
using MediaManager.Playback;
using MediaManager.Player;
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
    public event MediaItemChangedEventHandler? MediaItemChanged;

    
    private DataSourceItem? _dataSourceItem;
    private bool _blockEvents = false;


    public MediaPlayerService(AudiobookshelfDataSource dataSource, IMediaManager mediaManager)
    {

        _dataSource = dataSource;
        _mediaManager = mediaManager;
        _mediaManager.StateChanged += OnStateChanged;
        _mediaManager.PositionChanged += OnPositionChanged;
        _mediaManager.MediaItemChanged += OnMediaItemChanged;
        

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

    private void OnMediaItemChanged(object sender, MediaItemEventArgs e)
    {
        if (_blockEvents)
        {
            return;
        }
        MediaItemChanged?.Invoke(sender, e);
        _dataSource.HandleAction(_dataSourceItem, DataSourceAction.MediaItemChanged, e.MediaItem.Extras);
    }

    private void OnPositionChanged(object sender, PositionChangedEventArgs e)
    {
        if (_blockEvents)
        {
            return;
        }
        PositionChanged?.Invoke(sender, e);
        _dataSource.HandleAction(_dataSourceItem, DataSourceAction.PositionChanged, e.Position);
    }

    private void OnStateChanged(object sender, StateChangedEventArgs e)
    {
        if (_blockEvents)
        {
            return;
        }
        StateChanged?.Invoke(sender, e);
        var action = e.State switch
        {
            // handled in according wrapper methods
            // MediaPlayerState.Playing => DataSourceAction.Play,
            // MediaPlayerState.Paused => DataSourceAction.Pause,
            // MediaPlayerState.Stopped => DataSourceAction.Stop,
            
            MediaPlayerState.Loading => DataSourceAction.Loading,
            MediaPlayerState.Buffering => DataSourceAction.Buffering,
            MediaPlayerState.Failed => DataSourceAction.Fail,
            _ => DataSourceAction.None
        };

        if (action != DataSourceAction.None)
        {
            _dataSource.HandleAction(_dataSourceItem, action);
        }
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
        if (_dataSourceItem == item)
        {
            return;
        }
        
        if (_dataSourceItem != null)
        {
            _dataSource.HandleAction(_dataSourceItem, DataSourceAction.Remove);
        }

        _dataSourceItem = item;
        _dataSource.HandleAction(item, DataSourceAction.Insert);

        
        if (item.ActiveMediaItem == null)
        {
            return;
        }
            
        var mediaItems = item.MediaItems.Select(i => new MediaItem(i.Url.ToString())
        {
            Extras = i
        }).ToList();
        
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
            await _mediaManager.SeekTo(item.ActiveMediaItem.Position);
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
    
    public bool IsPlaying => _dataSourceItem?.Status == DataSourceItemStatus.Playing || _mediaManager.IsPlaying();

    public async Task StopAsync()
    {
        _dataSource.HandleAction(_dataSourceItem, DataSourceAction.Stop);
        await _mediaManager.Stop();
    }

    public async Task PlayAsync()
    {
        _dataSource.HandleAction(_dataSourceItem, DataSourceAction.Play);
        await _mediaManager.Play(); 
    }

    public async Task PauseAsync() 
    {
        _dataSource.HandleAction(_dataSourceItem, DataSourceAction.Pause);
        await _mediaManager.Pause(); 
    }

    public async Task SeekTo(TimeSpan t) {
        // handled via OnPositionChanged
        try
        {
            _blockEvents = true;
            _dataSource.HandleAction(_dataSourceItem, DataSourceAction.PositionChanged, t);
            await _mediaManager.SeekTo(t);
        }
        finally
        {
            _blockEvents = false;
        }

    }
    public Task Seek(TimeSpan offset) => SeekTo(_mediaManager.Position + offset);

    
    public Task NextChapter() => Seek(TimeSpan.FromMinutes(5));
    public Task PreviousChapter() =>  Seek(TimeSpan.FromMinutes(-5));
}
