using Avalonia.Preferences;
using ToneAudioPlayer.DataSources;

namespace ToneAudioPlayer.Services;

public class AppSettings: IAudiobookshelfSettings
{
    private readonly Preferences _prefs;

    public AppSettings(Preferences prefs)
    {
        _prefs = prefs;
        
    }

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
}