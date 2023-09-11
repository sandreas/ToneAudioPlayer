using System;
using System.Threading.Tasks;
using AudiobookshelfApi;
using AudiobookshelfApi.Api;
using AudiobookshelfApi.Responses;
using Avalonia.SimpleRouter;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ToneAudioPlayer.Services;

namespace ToneAudioPlayer.ViewModels;

public partial class SettingsViewModel: ViewModelBase
{
    private readonly HistoryRouter<ViewModelBase> _router;
    private readonly AppSettings _settings;
    private readonly Audiobookshelf _abs;

    [ObservableProperty]
    private string _url = "";
    [ObservableProperty]
    private string _username = "";
    [ObservableProperty]
    private string _password = "";
    
    [ObservableProperty]
    private string _keyLogContent = "";



    public SettingsViewModel(HistoryRouter<ViewModelBase> router, AppSettings settings, Audiobookshelf abs)
    {
        _router = router;
        _settings = settings;
        _abs = abs;
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
    private async Task Save()
    {

        var loginResponse = await _abs.LoginAsync(new Credentials { BaseAddress = new Uri(Url), Username = Username, Password = Password });
        if (loginResponse is ErrorResponse e)
        {
            return;
        }
        _settings.Url = Url;
        _settings.Username = Username;
        _settings.Password = Password;
        _router.GoTo<HomeViewModel>();
        
    }


}