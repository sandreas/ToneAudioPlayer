using CommunityToolkit.Mvvm.ComponentModel;
using ToneAudioPlayer.Services;

namespace ToneAudioPlayer.ViewModels;

public partial class MainViewModel : ViewModelBase
{
       
    [ObservableProperty]
    private ViewModelBase _content = default!;

    public MainViewModel(Router router)
    {
        router.CurrentViewModelChanged += viewModel => Content = viewModel;
        router.GoTo<HomeViewModel>();
    }
}
