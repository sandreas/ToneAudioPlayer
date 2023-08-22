using Avalonia.SimpleRouter;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SharpHook;

namespace ToneAudioPlayer.ViewModels;

public partial class SettingsViewModel: ViewModelBase
{
    private readonly HistoryRouter<ViewModelBase> _router;
    private readonly TaskPoolGlobalHook _sharpHook;

    [ObservableProperty]
    private string _keyLogContent = "";

    public SettingsViewModel(HistoryRouter<ViewModelBase> router, TaskPoolGlobalHook sharpHook)
    {
        _router = router;
        _sharpHook = sharpHook;
        _sharpHook.KeyTyped += OnKeyTyped;
        _sharpHook.KeyPressed += OnKeyPressed;
        _sharpHook.KeyReleased += OnKeyReleased;
    }

    private void OnKeyReleased(object? sender, KeyboardHookEventArgs e)
    {
        AppendKeyLog("KeyReleased", e);
    }

    private void AppendKeyLog(string keyreleased, KeyboardHookEventArgs e)
    {
        KeyLogContent += $"{keyreleased}: {e.Data.KeyChar}\n";
    }

    private void OnKeyTyped(object? sender, KeyboardHookEventArgs e)
    {
        AppendKeyLog("KeyTyped", e);
    }

    private void OnKeyPressed(object? sender, KeyboardHookEventArgs e)
    {
        AppendKeyLog("KeyPressed", e);
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