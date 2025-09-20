using AudioPlayerApp.ViewModels;

namespace AudioPlayerApp.Views;

public partial class PlaylistEditorPage : ContentPage
{
    public PlaylistEditorPage(PlaylistViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}