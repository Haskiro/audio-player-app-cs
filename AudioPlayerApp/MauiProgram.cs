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
            .UseMauiCommunityToolkit() // для CommunityToolkit.Controls и Behavior
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif
        // ===== Регистрация сервисов =====
        Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions
            .AddSingleton<IAudioService, IOSAudioService>(builder.Services);

        // ===== Регистрация ViewModels =====
        builder.Services.AddSingleton<PlayerViewModel>();
        builder.Services.AddSingleton<PlaylistViewModel>();

        // ===== Регистрация страниц =====
        builder.Services.AddSingleton<PlayerPage>();
        builder.Services.AddSingleton<PlaylistEditorPage>();

        // ===== Регистрация главной страницы (Shell) =====
        builder.Services.AddSingleton<AppShell>();

        return builder.Build();
    }
}