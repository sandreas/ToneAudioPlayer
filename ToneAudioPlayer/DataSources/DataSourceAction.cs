namespace ToneAudioPlayer.DataSources;

public enum DataSourceAction
{
    None,
    Remove,
    Insert,
    Play,
    Pause,
    Stop,
    MediaItemChanged,
    PositionChanged,
    
    Next,
    Previous,
    NextChapter,
    PreviousChapter,
    FastForward,
    Rewind,
    StepForward,
    StepBack,
    
    AddToFavorites,

    Loading,
    Buffering,
    Fail
}