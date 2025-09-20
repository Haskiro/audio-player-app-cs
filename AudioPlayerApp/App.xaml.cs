using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace AudioPlayerApp;

public partial class App : Application
{
    public App()
    {
        // Обработка непойманных исключений
        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            Exception ex = (Exception)args.ExceptionObject;
            System.Diagnostics.Debug.WriteLine($"Unhandled exception: {ex}");
        };

        TaskScheduler.UnobservedTaskException += (sender, args) =>
        {
            System.Diagnostics.Debug.WriteLine($"Unobserved task exception: {args.Exception}");
        };

        try
        {
            InitializeComponent();
            MainPage = new AppShell();
            System.Diagnostics.Debug.WriteLine("App initialized successfully");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"App initialization failed: {ex}");
            // В случае ошибки показываем простую страницу с сообщением
            MainPage = new ContentPage
            {
                Content = new Label { Text = $"Ошибка инициализации: {ex.Message}" }
            };
        }
    }
}