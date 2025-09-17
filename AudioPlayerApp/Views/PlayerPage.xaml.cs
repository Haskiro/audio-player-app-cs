using AudioPlayerApp.ViewModels;
using Microsoft.Maui.Controls;

namespace AudioPlayerApp.Views;

public partial class PlayerPage : ContentPage
{
    public PlayerPage(PlayerViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        
        if (BindingContext is PlayerViewModel viewModel)
        {
            viewModel.Dispose();
        }
    }
}