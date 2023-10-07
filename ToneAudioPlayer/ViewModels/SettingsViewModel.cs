using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using AudiobookshelfApi;
using AudiobookshelfApi.Api;
using AudiobookshelfApi.Responses;
using Avalonia.Platform.Storage;
using Avalonia.SimpleRouter;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ToneAudioPlayer.Models;
using ToneAudioPlayer.Services;

namespace ToneAudioPlayer.ViewModels;

public partial class SettingsViewModel: ViewModelBase, INotifyDataErrorInfo
{
    private readonly HistoryRouter<ViewModelBase> _router;
    private readonly AppSettings _settings;
    private readonly Audiobookshelf _abs;

    
    
    [ObservableProperty]
    private string _url = "";
    [ObservableProperty]
    private string _username = "";
    [ObservableProperty]
    private string _password = "";    
    [ObservableProperty]
    private string _storageFolder = "";
    
    [ObservableProperty]
    private bool _verifySettings = true;

    private List<DataValidationError> _errors = new();
    private readonly IStorageProvider _storage;

    public SettingsViewModel(IStorageProvider storage, HistoryRouter<ViewModelBase> router, AppSettings settings, Audiobookshelf abs)
    {
        _storage = storage;
        _router = router;
        _settings = settings;
        _abs = abs;
        Url = _settings.Url;
        Username = _settings.Username;
        Password = _settings.Password;
        StorageFolder = _settings.StorageFolder;
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
    
    [RelayCommand]
    private async Task Save()
    {
        try
        {
            if (VerifySettings)
            {
                var loginResponse = await _abs.LoginAsync(new Credentials { BaseAddress = new Uri(Url), Username = Username, Password = Password });
                if (loginResponse is ErrorResponse e)
                {
                    return;
                }
            }
            _settings.Url = Url;
            _settings.Username = Username;
            _settings.Password = Password;
            _settings.StorageFolder = StorageFolder;
            _router.GoTo<HomeViewModel>();
        }
        catch (Exception e)
        {
            UpdateErrors(e);
        }
        
    }

    /*
    private static T? GetValue<T>(params T?[] values) where T:class
    {
        return values.FirstOrDefault(v => v != null && EqualityComparer<T>.Default.Equals(v, default));
    }
    */
    private void UpdateErrors(Exception exception)
    {
        _errors.Add(new DataValidationError()
        {
            Message = exception.Message
        });
        HasErrors = true;
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(""));
    }

    public bool HasErrors { get; set; } = false;

    public IEnumerable GetErrors(string? propertyName)
    {
        return _errors.Where(e => e.PropertyName == propertyName).Select(e => e.Message);
    }

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;


    [RelayCommand]
    private async Task SelectStorageFolder()
    {
        var options = new FolderPickerOpenOptions
        {
            Title = "Select storage folder",
            AllowMultiple = false
        };
        
        var selectedFolder = await _storage.OpenFolderPickerAsync(options);

        StorageFolder = selectedFolder.FirstOrDefault()?.Path.ToString() ?? "";
    }
    
    
}