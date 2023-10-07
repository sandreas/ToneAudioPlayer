using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using MediaManager;
using MediaManager.Library;
using MediaManager.Queue;

namespace ToneAudioPlayer.MediaManagers;

public class DummyMediaQueue: List<IMediaItem>, IMediaQueue
{

    public event PropertyChangedEventHandler? PropertyChanged;
    public void Move(int oldIndex, int newIndex)
    {
        
    }

    public ObservableCollection<IMediaItem> MediaItems { get; }
    public string Title { get; set; }
    public bool HasNext { get; }
    public IMediaItem Next { get; }
    public bool HasPrevious { get; }
    public IMediaItem Previous { get; }
    public bool HasCurrent { get; }
    public int CurrentIndex { get; set; }
    public IMediaItem Current { get; }
    public event QueueEndedEventHandler? QueueEnded;
    public event QueueChangedEventHandler? QueueChanged;
}