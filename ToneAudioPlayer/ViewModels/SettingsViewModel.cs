using Avalonia.SimpleRouter;
using CommunityToolkit.Mvvm.Input;

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