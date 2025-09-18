using AudioPlayerApp.Services;
using AudioPlayerApp.ViewModels;
using AudioPlayerApp.Views;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using Microsoft.Maui.Hosting;

#if IOS
using AudioPlayerApp.Platforms.iOS.Services;
#endif

namespace AudioPlayerApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit() // если используешь CommunityToolkit
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // ===== Регистрация сервисов =====
#if IOS
        Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions
            .AddSingleton<IAudioService, IOSAudioService>(builder.Services);
#else
        Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions
            .AddSingleton<IAudioService, AudioService>(builder.Services);
#endif

        // ===== Регистрация ViewModels =====
        Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions
            .AddSingleton<PlayerViewModel>(builder.Services);
        Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions
            .AddSingleton<PlaylistViewModel>(builder.Services);

        // ===== Регистрация страниц =====
        Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions
            .AddSingleton<PlayerPage>(builder.Services);
        Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions
            .AddSingleton<PlaylistEditorPage>(builder.Services);

        // ===== Регистрация главной страницы (Shell) =====
        Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions
            .AddSingleton<AppShell>(builder.Services);

        return builder.Build();
    }
}
