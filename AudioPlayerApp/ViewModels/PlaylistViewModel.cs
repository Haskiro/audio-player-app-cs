using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using AudioPlayerApp.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;

namespace AudioPlayerApp.ViewModels;

public partial class PlaylistViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Playlist> _playlists = new();

    [ObservableProperty]
    private Playlist _selectedPlaylist;

    [ObservableProperty]
    private AudioTrack _selectedTrack;

    public PlaylistViewModel()
    {
        LoadPlaylists();
    }

    [RelayCommand]
    private async Task AddPlaylist()
    {
        var name = await Application.Current.MainPage.DisplayPromptAsync(
            "Новый плейлист",
            "Введите название плейлиста:",
            "Создать",
            "Отмена",
            "Мой плейлист");

        if (!string.IsNullOrWhiteSpace(name))
        {
            var playlist = new Playlist { Name = name };
            Playlists.Add(playlist);
            SavePlaylists();
        }
    }

    [RelayCommand]
    private void DeletePlaylist(Playlist playlist)
    {
        Playlists.Remove(playlist);
        SavePlaylists();
    }

    [RelayCommand]
    private async Task AddTrackToPlaylist(Playlist playlist)
    {
        try
        {
            var customFileType = new FilePickerFileType(
                new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.iOS, new[] { "public.audio" } }, // UTType для аудио
                });

            var options = new PickOptions
            {
                PickerTitle = "Выберите аудиофайлы",
                FileTypes = customFileType
            };

            var results = await FilePicker.Default.PickMultipleAsync(options);

            if (results == null) return;

            foreach (var file in results)
            {
                var track = new AudioTrack
                {
                    FilePath = file.FullPath,
                    Title = System.IO.Path.GetFileNameWithoutExtension(file.FileName),
                    Artist = "Неизвестный исполнитель",
                    Duration = TimeSpan.Zero // В реальном приложении можно получить длительность из метаданных
                };

                playlist.Tracks.Add(track); // ObservableCollection → UI обновится
                playlist.ModifiedDate = DateTime.Now;
            }

            SavePlaylists();
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Ошибка", $"Не удалось добавить треки: {ex.Message}", "OK");
        }
    }
    
    [RelayCommand]
    private void RemoveTrackFromPlaylist(AudioTrack track)
    {
        if (SelectedPlaylist != null && track != null)
        {
            SelectedPlaylist.Tracks.Remove(track);
            SelectedPlaylist.ModifiedDate = DateTime.Now;
            SavePlaylists();
        }
    }

    [RelayCommand]
    private async Task EditPlaylistName(Playlist playlist)
    {
        var newName = await Application.Current.MainPage.DisplayPromptAsync(
            "Редактирование",
            "Введите новое название плейлиста:",
            "Сохранить",
            "Отмена",
            playlist.Name);

        if (!string.IsNullOrWhiteSpace(newName))
        {
            playlist.Name = newName;
            playlist.ModifiedDate = DateTime.Now;
            SavePlaylists();
        }
    }

    private void LoadPlaylists()
    {
        // Пример начальных плейлистов
        Playlists = new ObservableCollection<Playlist>
        {
            new Playlist { Name = "Мои треки" },
            new Playlist { Name = "Избранное" }
        };
    }

    private void SavePlaylists()
    {
        // В реальном приложении реализовать сохранение в файл/базу данных
    }
}
