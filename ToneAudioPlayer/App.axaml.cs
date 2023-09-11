using System;
using AudiobookshelfApi;
using AudiobookshelfApi.Api;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Preferences;
using Avalonia.SimpleRouter;
using MediaManager;
using Microsoft.Extensions.DependencyInjection;
using ToneAudioPlayer.DataSources;
using ToneAudioPlayer.MediaManagers;
using ToneAudioPlayer.Services;
using ToneAudioPlayer.ViewModels;
using ToneAudioPlayer.Views;

namespace ToneAudioPlayer;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        IServiceProvider services = ConfigureServices();



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
    }


    private static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();
        services.AddSingleton<Preferences>();
        services.AddSingleton<AppSettings>();

        services.AddSingleton<IAudiobookshelfSettings>(s => s.GetRequiredService<AppSettings>());
        services.AddSingleton<AudiobookshelfDataSource>();

        services.AddSingleton<HistoryRouter<ViewModelBase>>(s =>
            new HistoryRouter<ViewModelBase>(t => (ViewModelBase)s.GetRequiredService(t)));

        services.AddSingleton<Credentials>(s =>
        {
            var settings = s.GetRequiredService<AppSettings>();
            return new Credentials
            {
                BaseAddress = string.IsNullOrEmpty(settings.Url) ? new Uri("") : new Uri(settings.Url),
                Username = settings.Username,
                Password = settings.Password
            };
            
        });

        services.AddSingleton<MainViewModel>();

        services.AddTransient<HomeViewModel>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<SearchViewModel>();

        services.AddSingleton(s =>
        {
            IMediaManager mgr;
            try
            {
                mgr = CrossMediaManager.Current;
            }
            catch (Exception)
            {
                mgr = new DummyMediaManager();
            }
            
            return new MediaPlayerService(s.GetRequiredService<AudiobookshelfDataSource>(), mgr);

        });
            
        
        // services.AddSingleton<Audiobookshelf>();
        services.AddHttpClient<Audiobookshelf>()
            .ConfigureHttpClient((_, httpClient) => { httpClient.Timeout = TimeSpan.FromSeconds(10); })
            .SetHandlerLifetime(TimeSpan.FromMinutes(5));

        return services.BuildServiceProvider();
    }
}