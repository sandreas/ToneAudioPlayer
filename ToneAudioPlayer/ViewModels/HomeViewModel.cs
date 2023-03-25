using CommunityToolkit.Mvvm.Input;
using ToneAudioPlayer.Services;

namespace ToneAudioPlayer.ViewModels;

public partial class HomeViewModel: ViewModelBase
{
    private readonly Router<ViewModelBase> _router;

    public HomeViewModel(Router<ViewModelBase> router)
    {
        _router = router;
    }
    
    [RelayCommand]
    private void Navigate(string parameter = "")
    {
        switch (parameter)
        {
            case "music":
                // _router.GoTo<>();
                break;
            case "settings":
                _router.GoTo<SettingsViewModel>();
                break;
        }
    }
}