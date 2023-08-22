using System;
using Avalonia.SimpleRouter;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ToneAudioPlayer.ViewModels;

public partial class MainViewModel : ViewModelBase
{
       
    [ObservableProperty]
    private ViewModelBase _content = default!;

    public MainViewModel(HistoryRouter<ViewModelBase> router)
    {
        router.CurrentViewModelChanged += viewModel => Content = viewModel;
        router.GoTo<HomeViewModel>();
    }

    [RelayCommand]
    private void ShortCutPressed(int parameter)
    {
        Console.WriteLine($"do something {parameter}");
    }
}
