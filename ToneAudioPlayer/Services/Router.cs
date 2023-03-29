namespace ToneAudioPlayer.Services;

using System;
using Microsoft.Extensions.DependencyInjection;

public class Router<TViewModelBase> where TViewModelBase:class
{

    private TViewModelBase _currentViewModel = default!;
    protected readonly IServiceProvider Services;
    public event Action<TViewModelBase>? CurrentViewModelChanged;

    public Router(IServiceProvider services)
    {
        Services = services;
    }

    protected TViewModelBase CurrentViewModel
    {
        set
        {
            if (value == _currentViewModel) return;
            _currentViewModel = value;
            OnCurrentViewModelChanged(value);
        }
    }

    private void OnCurrentViewModelChanged(TViewModelBase viewModel)
    {
        CurrentViewModelChanged?.Invoke(viewModel);
    }

    public virtual void GoTo<T>() where T : TViewModelBase
    {
        CurrentViewModel = Services.GetRequiredService<T>();
    }

}