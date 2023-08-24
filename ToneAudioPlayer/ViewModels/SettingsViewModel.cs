using AudiobookshelfApi.Models;
using Avalonia.SimpleRouter;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ToneAudioPlayer.Services;

namespace ToneAudioPlayer.ViewModels;

public partial class SettingsViewModel: ViewModelBase
{
    private readonly HistoryRouter<ViewModelBase> _router;
    private readonly AppSettings _settings;

    [ObservableProperty]
    private string _url = "";
    [ObservableProperty]
    private string _username = "";
    [ObservableProperty]
    private string _password = "";
    
    [ObservableProperty]
    private string _keyLogContent = "";


    public SettingsViewModel(HistoryRouter<ViewModelBase> router, AppSettings settings)
    {
        _router = router;
        _settings = settings;
        Url = _settings.Url;
        Username = _settings.Username;
        Password = _settings.Password;
    }

    [RelayCommand]
    private void Navigate(string parameter = "")
    {
        switch (parameter)
        {
            case "back":
                _router.Back();
                break;
        }
    }
    
    [RelayCommand]
    private void Save()
    {
        _settings.Url = Url;
        _settings.Username = Username;
        _settings.Password = Password;
    }


}