using System;
using AudiobookshelfApi;
using AudiobookshelfApi.Api;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.SimpleRouter;
using LibVLCSharp.Shared;
using Microsoft.Extensions.DependencyInjection;
using SharpHook;
using ToneAudioPlayer.Services;
using ToneAudioPlayer.ViewModels;
using ToneAudioPlayer.Views;

namespace ToneAudioPlayer;

public class App : Application
{

    private TaskPoolGlobalHook _sharpHook = new();
    private MediaPlayerService _mediaPlayerService;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
            
        IServiceProvider services = ConfigureServices();

        _sharpHook = new TaskPoolGlobalHook();
        _sharpHook.KeyTyped += OnKeyTyped;
        _sharpHook.KeyPressed += OnKeyPressed;
        _sharpHook.KeyReleased += OnKeyReleased;
        _sharpHook.RunAsync();

        _mediaPlayerService = services.GetRequiredService<MediaPlayerService>();
        
        Core.Initialize();

        var mainViewModel = services.GetRequiredService<MainViewModel>();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = mainViewModel,
            };

            desktop.MainWindow.Closing += OnClosing;
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = mainViewModel
            };
            singleViewPlatform.MainView.Unloaded += OnUnloaded;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void OnUnloaded(object? sender, RoutedEventArgs e)
    {
        CleanUpBeforeExit();
    }

    private void OnClosing(object? sender, WindowClosingEventArgs e)
    {
        CleanUpBeforeExit();
    }

    private void CleanUpBeforeExit()
    {
        _mediaPlayerService.Dispose();
        _sharpHook.Dispose();
    }


    private static ServiceProvider ConfigureServices()
    {

        
        var services = new ServiceCollection();
        
        services.AddSingleton<HistoryRouter<ViewModelBase>>(s => new HistoryRouter<ViewModelBase>(t => (ViewModelBase)s.GetRequiredService(t)));

        services.AddSingleton<Credentials>(_ => new Credentials
        {
            BaseAddress = new Uri(Environment.GetEnvironmentVariable("AUDIOBOOKSHELF_URL") ?? ""),
            Username = Environment.GetEnvironmentVariable("AUDIOBOOKSHELF_USERNAME") ?? "root",
            Password = Environment.GetEnvironmentVariable("AUDIOBOOKSHELF_PASSWORD") ?? ""
        });
        services.AddSingleton<LibVLC>(s => new LibVLC(enableDebugLogs:false));
        services.AddSingleton<MediaPlayer>();
        services.AddSingleton<MediaPlayerService>();

        services.AddSingleton<MainViewModel>();
        
        services.AddTransient<HomeViewModel>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<SearchViewModel>();
        
        // services.AddSingleton<Audiobookshelf>();
        services.AddHttpClient<Audiobookshelf>()
            .ConfigureHttpClient((sp, httpClient) =>
            {
                httpClient.Timeout = TimeSpan.FromSeconds(10);
            })
            .SetHandlerLifetime(TimeSpan.FromMinutes(5));

        return services.BuildServiceProvider();
    }
    
    
    private static void OnKeyReleased(object? sender, KeyboardHookEventArgs e)
    {
        AppendKeyLog("KeyReleased", e);
    }

    private static void AppendKeyLog(string keyreleased, KeyboardHookEventArgs e)
    {
        KeyLogContent += $"{keyreleased}: {e.Data.KeyChar}\n";
    }

    public static string KeyLogContent { get; set; } = "";

    private static void OnKeyTyped(object? sender, KeyboardHookEventArgs e)
    {
        AppendKeyLog("KeyTyped", e);
    }

    private static void OnKeyPressed(object? sender, KeyboardHookEventArgs e)
    {
        AppendKeyLog("KeyPressed", e);
    }
    
    
}