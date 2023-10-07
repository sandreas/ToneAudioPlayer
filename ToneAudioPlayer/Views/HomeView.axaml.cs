using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ToneAudioPlayer.Views;

public partial class HomeView : UserControl
{
    public HomeView()
    {
        InitializeComponent();
        var searchQueryControl = this.FindControl<TextBox>("Query");
        if (searchQueryControl != null)
        {
            searchQueryControl.AttachedToVisualTree += (s,e) => searchQueryControl.Focus();
        }
    }
}