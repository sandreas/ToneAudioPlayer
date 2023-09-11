using System;
using Avalonia.SimpleRouter;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediaManager.Playback;
using MediaManager.Player;
using ToneAudioPlayer.Services;

namespace ToneAudioPlayer.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly HistoryRouter<ViewModelBase> _router;
    private readonly AppSettings _settings;
    private readonly MediaPlayerService _player;

    [ObservableProperty]
    private ViewModelBase _content = default!;
    
    [ObservableProperty]
    private bool _isMenuVisible;

        
    [ObservableProperty]
    private bool _hasNextRoute;
    
    [ObservableProperty]
    private bool _hasPreviousRoute;


    
    
    [ObservableProperty]
    private string _currentPosition = "";
    [ObservableProperty]
    private string _totalDuration = "";
    [ObservableProperty]
    private bool _isPlaying = false;

    

    public MainViewModel(HistoryRouter<ViewModelBase> router, AppSettings settings, MediaPlayerService player)
    {
        _router = router;
        _settings = settings;
        _player = player;
        
        router.CurrentViewModelChanged += viewModel =>
        {
            Content = viewModel;
            HasPreviousRoute = router.HasPrev;
            HasNextRoute = router.HasNext;
        };

        _player.StateChanged += OnStateChanged;
        _player.PositionChanged += OnPositionChanged;

        if (settings.IsConfigured)
        {
            router.GoTo<HomeViewModel>();
        }
        else
        {
            router.GoTo<SettingsViewModel>();
            IsMenuVisible = false;
        }
    }

    private void OnPositionChanged(object sender, PositionChangedEventArgs e)
    {
        CurrentPosition = FormatTimeSpan(e.Position);
        TotalDuration = FormatTimeSpan(_player.Duration);
        
    }

    private static string FormatTimeSpan(TimeSpan span) => $"{(int)span.TotalHours:D2}:{span.Minutes:D2}:{span.Seconds:D2}";

    private void OnStateChanged(object sender, StateChangedEventArgs e)
    {
        if (e.State == MediaPlayerState.Playing)
        {
            IsPlaying = true;
        }
    }

    [RelayCommand]
    private void ToggleMenu() => IsMenuVisible ^= true;

    [RelayCommand]
    private void Navigate(object destination)
    {
        if (destination is not string dirString)
        {
            return;
        }

        var route = destination switch {
            "back" => _router.Back(),
            "forward" => _router.Forward(),
            "home" => _router.GoTo<HomeViewModel>(),
            "settings" => _router.GoTo<SettingsViewModel>(),
            _ => _router.Go(0)
        };
        
        ToggleMenu();
        
    }

}
