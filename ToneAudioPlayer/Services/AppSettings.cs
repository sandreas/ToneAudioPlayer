using Avalonia.Preferences;
using ToneAudioPlayer.DataSources;
using ToneAudioPlayer.DataSources.Audiobookshelf;
using ToneAudioPlayer.DataSources.Local;

namespace ToneAudioPlayer.Services;

public class AppSettings: ILocalDataSourceSettings
{
    private readonly Preferences _prefs;

    public AppSettings(Preferences prefs)
    {
        
        _prefs = prefs;
    }

    public bool IsConfigured => !string.IsNullOrEmpty(Url);
    
    public string Url
    {
        get => _prefs.Get(nameof(Url), "") ?? ""; 
        set => _prefs.Set(nameof(Url), value);
    }

    public string Username
    {
        get => _prefs.Get(nameof(Username), "") ?? ""; 
        set => _prefs.Set(nameof(Username), value);
    }
    
    public string Password
    {
        get => _prefs.Get(nameof(Password), "") ?? ""; 
        set => _prefs.Set(nameof(Password), value);
    }
    
    public string StorageFolder
    {
        get => _prefs.Get(nameof(StorageFolder), "") ?? ""; 
        set => _prefs.Set(nameof(StorageFolder), value);
    }

}