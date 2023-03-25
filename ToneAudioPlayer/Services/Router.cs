using System.Collections.Generic;
using System.Linq;

namespace ToneAudioPlayer.Services;

using System;
using Microsoft.Extensions.DependencyInjection;

public class Router<TBaseViewModel> where TBaseViewModel:class
{

    private TBaseViewModel _currentViewModel = default!;
    private readonly IServiceProvider _services;
    public event Action<TBaseViewModel>? CurrentViewModelChanged;


    
    private int _historyIndex = -1;
    private List<TBaseViewModel> _history = new();
    private readonly uint _historyMaxSize = 100;

    public bool HasNext => _history.Count > 0 && _historyIndex < _history.Count - 1;
    public bool HasPrev => _historyIndex > 0;

    // pushState
    // popState
    // replaceState

    public void Push(TBaseViewModel item)
    {
        if (HasNext)
        {
            _history = _history.Take(_historyIndex + 1).ToList();
        }
        _history.Add(item);
        _historyIndex = _history.Count - 1;
        if (_history.Count > _historyMaxSize)
        {
            _history.RemoveAt(0);
        }
    }
    
    public void Go(int offset = 0)
    {
        
        if (offset == 0)
        {
            return;
        }

        var newIndex = _historyIndex + offset;
        if (newIndex < 0 || newIndex > _history.Count - 1)
        {
            return;
        }
        _historyIndex = newIndex;
        CurrentViewModel = _history.ElementAt(_historyIndex);
    }
    
    public void Back()
    {
        if (HasPrev)
        {
            Go(-1);
        }
    }
    public void Forward()
    {
        if (HasNext)
        {
            Go(1);
        }
    }    
    
    public Router(IServiceProvider services)
    {
        _services = services;
    }

    private TBaseViewModel CurrentViewModel
    {
        set
        {
            if (value != _currentViewModel)
            {
                _currentViewModel = value;
                OnCurrentViewModelChanged(value);
            }
        }
    }

    private void OnCurrentViewModelChanged(TBaseViewModel viewModel)
    {
        CurrentViewModelChanged?.Invoke(viewModel);
    }

    public void GoTo<T>() where T : TBaseViewModel
    {
        var destination = _services.GetRequiredService<T>();
        CurrentViewModel = destination;
        Push(destination);
    }

}