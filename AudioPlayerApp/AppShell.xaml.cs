using AudioPlayerApp.Views;
using Microsoft.Maui.Controls;

namespace AudioPlayerApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        
        // Регистрация маршрутов
        Routing.RegisterRoute(nameof(PlayerPage), typeof(PlayerPage));
        Routing.RegisterRoute(nameof(PlaylistEditorPage), typeof(PlaylistEditorPage));
    }
}