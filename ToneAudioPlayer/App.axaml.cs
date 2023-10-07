using System;
using System.IO;
using AudiobookshelfApi;
using AudiobookshelfApi.Api;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Avalonia.Preferences;
using Avalonia.SimpleRouter;
using LiteDB;
using MediaManager;
using Microsoft.Extensions.DependencyInjection;
using ToneAudioPlayer.DataSources;
using ToneAudioPlayer.DataSources.Audiobookshelf;
using ToneAudioPlayer.DataSources.Local;
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
        switch (ApplicationLifetime)
        {
            case IClassicDesktopStyleApplicationLifetime desktop:
                desktop.MainWindow = new MainWindow();
                InitMainControl(desktop.MainWindow);
                // desktop.MainWindow.Closing += OnClosing;
                break;
            case ISingleViewApplicationLifetime singleViewPlatform:
                singleViewPlatform.MainView = new MainView();
                InitMainControl(singleViewPlatform.MainView);
                // singleViewPlatform.MainView.Unloaded += OnUnloaded;
                break;
            default:
                throw new PlatformNotSupportedException("ApplicationLifetime has do be SingleView or ClassicDesktop");
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void InitMainControl(Control mainControl)
    {
        IServiceProvider services = ConfigureServices(mainControl);
        var mainViewModel = services.GetRequiredService<MainViewModel>();
        mainControl.DataContext = mainViewModel;
    }


    private static ServiceProvider ConfigureServices(Visual mainControl)
    {
        var services = new ServiceCollection();
        services.AddSingleton<Preferences>();
        services.AddSingleton<AppSettings>();

        services.AddSingleton<ILocalDataSourceSettings>(s => s.GetRequiredService<AppSettings>());
        services.AddSingleton<AudiobookshelfDataSource>();
        services.AddSingleton<ILiteDatabase>(s =>
        {
            var appSettings = s.GetRequiredService<AppSettings>();
            if (!Directory.Exists(appSettings.StorageFolder))
            {
                Directory.CreateDirectory(appSettings.StorageFolder);
            }
            return new LiteDatabase(Path.Combine(appSettings.StorageFolder, "lite.db"));
        });
        
        services.AddSingleton<LocalDataSource>(s =>
        {
            var appSettings = s.GetRequiredService<AppSettings>();
            var liteDb = s.GetRequiredService<ILiteDatabase>();
            return new LocalDataSource(appSettings, liteDb);
        });

        services.AddSingleton<HistoryRouter<ViewModelBase>>(s =>
            new HistoryRouter<ViewModelBase>(t => (ViewModelBase)s.GetRequiredService(t)));

        services.AddSingleton<Credentials>(s =>
        {
            var settings = s.GetRequiredService<AppSettings>();

            return new Credentials
            {
                BaseAddress = string.IsNullOrEmpty(settings.Url) ? new Uri("about:blank") : new Uri(settings.Url),
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
            .ConfigureHttpClient((_, httpClient) => { httpClient.Timeout = TimeSpan.FromSeconds(5); })
            .SetHandlerLifetime(TimeSpan.FromMinutes(5));

        services.AddHttpClient<DownloadService>()
            .ConfigureHttpClient((_, httpClient) => { httpClient.Timeout = TimeSpan.FromSeconds(5); })
            .SetHandlerLifetime(TimeSpan.FromMinutes(5));
        
        services.AddSingleton<IStorageProvider>(_ => TopLevel.GetTopLevel(mainControl)?.StorageProvider!);
        
        return services.BuildServiceProvider();
    }
}