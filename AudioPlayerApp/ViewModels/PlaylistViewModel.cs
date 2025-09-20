using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using AudioPlayerApp.Models;
using AudioPlayerApp.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;

namespace AudioPlayerApp.ViewModels
{
    public partial class PlaylistViewModel : ObservableObject
    {
        private readonly AppStateService _appStateService;
        private readonly IAudioService _audioService;
        private readonly FileSystemDataService _dataService;
        public PlayerViewModel PlayerViewModel { get; }

        [ObservableProperty]
        private ObservableCollection<Playlist> _playlists = new();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsPlaylistSelected))]
        private Playlist _selectedPlaylist;

        [ObservableProperty]
        private AudioTrack _selectedTrack;

        public bool IsPlaylistSelected => SelectedPlaylist != null;

        public PlaylistViewModel(AppStateService appStateService, IAudioService audioService, FileSystemDataService dataService)
        {
            _appStateService = appStateService;
            _audioService = audioService;
            _audioService = audioService;
            _dataService = dataService;
            PlayerViewModel = new PlayerViewModel(_audioService, _appStateService);

            LoadPlaylists();
        }
        
        
        private async void LoadPlaylists()
        {
            Playlists = await _dataService.LoadPlaylistsAsync();
    
            // Если нет плейлистов, загружаем демо-данные
            if (Playlists.Count == 0)
            {
                Playlists = new ObservableCollection<Playlist>
                {
                    new Playlist
                    {
                        Name = "Мои треки",
                        Tracks = new ObservableCollection<AudioTrack>(),
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now
                    }
                };
            }
    
            // Автоматически выберем первый плейлист
            if (Playlists.Count > 0)
            {
                SelectedPlaylist = Playlists[0];
            }
        }

        #region Commands (RelayCommand methods)

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
                var playlist = new Playlist
                {
                    Name = name,
                    Tracks = new ObservableCollection<AudioTrack>(),
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now
                };

                Playlists.Add(playlist);
                SelectedPlaylist = playlist; // автоматически выбираем новый плейлист

                SavePlaylists();
            }
        }

        [RelayCommand]
        private void DeletePlaylist(Playlist playlist)
        {
            if (playlist == null) return;

            bool removed = Playlists.Remove(playlist);
            if (removed && SelectedPlaylist == playlist)
            {
                SelectedPlaylist = null;
            }
            
            SavePlaylists();
        }

        [RelayCommand]
        private async Task AddTrackToPlaylist(Playlist playlist)
        {
            if (playlist == null)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Сначала выберите плейлист", "OK");
                return;
            }

            try
            {
                var customFileType = new FilePickerFileType(
                    new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.iOS, new[] { "public.audio" } },
                        { DevicePlatform.Android, new[] { "audio/*" } },
                        { DevicePlatform.WinUI, new[] { ".mp3", ".wav", ".m4a", ".aac" } }
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
                    try
                    {
                        double durationInSeconds = 0;
                        try
                        {
                            durationInSeconds = _audioService.GetTrackDuration(file.FullPath);
                        }
                        catch
                        {
                            durationInSeconds = 0;
                        }

                        var track = new AudioTrack
                        {
                            FilePath = file.FullPath,
                            Title = Path.GetFileNameWithoutExtension(file.FileName),
                            Album = "Неизвестный альбом",
                            Duration = TimeSpan.FromSeconds(durationInSeconds)  
                        };

                        playlist.Tracks.Add(track);
                        playlist.ModifiedDate = DateTime.Now;

                        SavePlaylists();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error adding track: {ex.Message}");
                    }
                }
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
            if (playlist == null) return;

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

        [RelayCommand]
        private void SelectPlaylist(Playlist playlist)
        {
            if (playlist == null) return;

            SelectedPlaylist = playlist;
            _appStateService.SetCurrentPlaylist(playlist);

            Console.WriteLine($"Playlist selected: {playlist.Name}");
            Console.WriteLine($"Tracks count: {playlist.Tracks?.Count ?? 0}");
        }

        [RelayCommand]
        private async Task SelectTrack(AudioTrack track)
        {
            if (SelectedPlaylist == null)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Сначала выберите плейлист", "OK");
                return;
            }

            if (track == null) return;

            _appStateService.SetCurrentPlaylist(SelectedPlaylist);
            _appStateService.SetCurrentTrack(track);
    
            PlayerViewModel.PlayTrack(track);
            await Shell.Current.GoToAsync($"//{nameof(PlayerPage)}");
        }


        #endregion

        #region Helpers: Load/Save

        partial void OnSelectedPlaylistChanged(Playlist value)
        {
            Console.WriteLine($"SelectedPlaylist changed to: {value?.Name}");
            if (value != null)
            {
                Console.WriteLine($"Tracks in selected playlist: {value.Tracks?.Count ?? 0}");
            }
        }
        
        private async void SavePlaylists()
        {
            await _dataService.SavePlaylistsAsync(Playlists);
        }

        #endregion
    }
}
