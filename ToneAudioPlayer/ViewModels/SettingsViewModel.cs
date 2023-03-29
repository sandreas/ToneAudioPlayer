using CommunityToolkit.Mvvm.Input;
using ToneAudioPlayer.Services;

namespace ToneAudioPlayer.ViewModels;

public partial class SettingsViewModel: ViewModelBase
{
    private readonly HistoryRouter<ViewModelBase> _router;

    public SettingsViewModel(HistoryRouter<ViewModelBase> router)
    {
        _router = router;
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
}