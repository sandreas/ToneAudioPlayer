using ToneAudioPlayer.ViewModels;

namespace ToneAudioPlayer.Services;

using System;
using Microsoft.Extensions.DependencyInjection;


public class Router
{
    
    private ViewModelBase _currentViewModel = default!;
    private readonly IServiceProvider _services;
    public event Action<ViewModelBase>? CurrentViewModelChanged;
    
    public Router(IServiceProvider services)
    {
        _services = services;
    }

    private ViewModelBase CurrentViewModel
    {
        set {
            if (value != _currentViewModel)
            {
                _currentViewModel = value;
                OnCurrentViewModelChanged(value);
            }
        }
    }

    private void OnCurrentViewModelChanged(ViewModelBase viewModel)
    {
        CurrentViewModelChanged?.Invoke(viewModel);
    }

    public void GoTo<T>() where T : ViewModelBase
    {
        CurrentViewModel = _services.GetRequiredService<T>();
    }
}