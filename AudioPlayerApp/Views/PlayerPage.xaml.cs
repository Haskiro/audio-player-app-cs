using System;
using AudioPlayerApp.ViewModels;
using Microsoft.Maui.Controls;

namespace AudioPlayerApp.Views;

public partial class PlayerPage : ContentPage
{
    private PlayerViewModel _viewModel;
    
    public PlayerPage(PlayerViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        // Обновляем данные при переходе на страницу
        if (_viewModel != null)
        {
            _viewModel.CurrentTrack = _viewModel.CurrentTrack; // Принудительное обновление
        }
    }
    
    private void OnSliderDragStarted(object sender, EventArgs e)
    {
        _viewModel.StartDragging();
    }
    
    private void OnSliderDragCompleted(object sender, EventArgs e)
    {
        _viewModel.EndDragging();
    }
}