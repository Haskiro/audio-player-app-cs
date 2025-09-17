using AudioPlayerApp.ViewModels;
using Microsoft.Maui.Controls;

namespace AudioPlayerApp.Views;

public partial class PlaylistEditorPage : ContentPage
{
    public PlaylistEditorPage(PlaylistViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}