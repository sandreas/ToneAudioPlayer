﻿using System.Runtime.Versioning;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using ToneAudioPlayer;

[assembly: SupportedOSPlatform("browser")]

internal partial class Program
{
    private static async Task Main(string[] args) => await BuildAvaloniaApp()
        .SetupBrowserAppAsync(new BrowserPlatformOptions());

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>();
}