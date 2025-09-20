using AudioPlayerApp.Models;
using AudioPlayerApp.Platforms.iOS.Services;
using AudioPlayerApp.ViewModels;
using AudioPlayerApp.Views;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using Microsoft.Maui.Hosting;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls.Hosting;

// using Microsoft.Maui.Controls.Hosting;

namespace AudioPlayerApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif
        // Регистрируем коллекцию треков
        builder.Services.AddSingleton<ObservableCollection<AudioTrack>>();
        // Регистрация сервисов
        builder.Services.AddSingleton<IAudioService, IOSAudioService>();
        builder.Services.AddSingleton<AppStateService>();
        builder.Services.AddSingleton<FileSystemDataService>(); 

        // Регистрация ViewModels
        builder.Services.AddSingleton<PlaylistViewModel>();
        builder.Services.AddSingleton<PlayerViewModel>();

        // Регистрация страниц
        builder.Services.AddTransient<PlaylistEditorPage>();
        builder.Services.AddTransient<PlayerPage>();

        return builder.Build();
    }
}