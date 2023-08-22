using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace ToneAudioPlayer.Views;

public partial class SettingsView : UserControl
{
    public SettingsView()
    {
        InitializeComponent();
    }

    /*
    protected override void OnKeyDown(KeyEventArgs e)
    {
        KeyLog.Text += $"OnKeyDown: key={e.Key}\n";
        base.OnKeyDown(e);
    }
    */
}