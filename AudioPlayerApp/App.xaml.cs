using Microsoft.Maui.Controls;

namespace AudioPlayerApp;

public partial class App : Application
{
    public App(AppShell shell)
    {
        InitializeComponent();
        MainPage = shell;
    }
}