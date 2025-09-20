using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AudioPlayerApp.Models;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;

namespace AudioPlayerApp.ViewModels
{
    public partial class PlayerViewModel : ObservableObject
    {
        private readonly IAudioService _audioService;
        private readonly AppStateService _appStateService;
        private bool _isUserDraggingSlider;
        private IDispatcherTimer _updateTimer;

        [ObservableProperty]
        private AudioTrack _currentTrack;

        [ObservableProperty]
        private TimeSpan _position;

        [ObservableProperty]
        private TimeSpan _duration;

        [ObservableProperty]
        private bool _isPlaying = true;

        [ObservableProperty]
        private double _playbackSpeed = 1.0;

        [ObservableProperty]
        private string _currentTimeFormatted = "00:00";

        [ObservableProperty]
        private string _totalTimeFormatted = "00:00";

        [ObservableProperty]
        private double _sliderPosition;

        public PlayerViewModel(IAudioService audioService, AppStateService appStateService)
        {
            _audioService = audioService;
            _appStateService = appStateService;

            // Инициализация текущего трека
            CurrentTrack = _appStateService.CurrentTrack;
            
            // Подписка на изменения текущего трека
            _appStateService.CurrentTrackChanged += OnCurrentTrackChanged;

            // Создаем таймер для обновления позиции
            _updateTimer = Application.Current.Dispatcher.CreateTimer();
            _updateTimer.Interval = TimeSpan.FromMilliseconds(100);
            _updateTimer.Tick += OnUpdateTimerTick;
            _updateTimer.Start();
        }

        private void OnUpdateTimerTick(object sender, EventArgs e)
        {
            if (_isPlaying && !_isUserDraggingSlider && CurrentTrack != null)
            {
                var currentPos = _audioService.GetCurrentPosition();
                Position = TimeSpan.FromSeconds(currentPos);
                CurrentTimeFormatted = FormatTime(Position);
                SliderPosition = currentPos;
            }
        }

        private void OnCurrentTrackChanged(object sender, AudioTrack track)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                CurrentTrack = track;
                
                if (track != null)
                {
                    // Загружаем длительность трека
                    var duration = _audioService.GetTrackDuration(track.FilePath);
                    if (duration > 0)
                    {
                        Duration = TimeSpan.FromSeconds(duration);
                        TotalTimeFormatted = FormatTime(Duration);
                    }
                    
                    // Сбрасываем позицию
                    Position = TimeSpan.Zero;
                    CurrentTimeFormatted = "00:00";
                    SliderPosition = 0;
                }
            });
        }

        public string FormatTime(TimeSpan time)
        {
            return time.ToString(@"mm\:ss");
        }

        partial void OnSliderPositionChanged(double value)
        {
            if (_isUserDraggingSlider)
            {
                // Обновляем отображаемое время при перетаскивании слайдера
                Position = TimeSpan.FromSeconds(value);
                CurrentTimeFormatted = FormatTime(Position);
            }
        }

        [RelayCommand]
        public void PlayPause()
        {
            if (IsPlaying)
            {
                Pause();
            }
            else
            {
                Play();
            }
        }

        public void Play()
        {
            if (CurrentTrack != null)
            {
                _audioService.Play(CurrentTrack.FilePath);
                IsPlaying = true;
            }
        }

        public void Pause()
        {
            _audioService.Pause();
            IsPlaying = false;
        }

        [RelayCommand]
        public void Stop()
        {
            _audioService.Stop();
            IsPlaying = false;
            Position = TimeSpan.Zero;
            CurrentTimeFormatted = "00:00";
            SliderPosition = 0;
        }

        [RelayCommand]
        public void SeekForward()
        {
            var newPosition = _audioService.GetCurrentPosition() + 10;
            _audioService.Seek(newPosition);
        }

        [RelayCommand]
        public void SeekBackward()
        {
            var newPosition = _audioService.GetCurrentPosition() - 10;
            if (newPosition < 0) newPosition = 0;
            _audioService.Seek(newPosition);
        }

        [RelayCommand]
        public void ChangeSpeed()
        {
            var speeds = new[] { 0.5, 0.75, 1.0, 1.25, 1.5, 2.0 };
            var currentIndex = Array.IndexOf(speeds, PlaybackSpeed);
            var nextIndex = (currentIndex + 1) % speeds.Length;
            PlaybackSpeed = speeds[nextIndex];
            
            _audioService.SetSpeed(PlaybackSpeed);
        }

        [RelayCommand]
        public void PlayNext()
        {
            var nextTrack = _appStateService.GetNextTrack();
            if (nextTrack != null)
            {
                PlayTrack(nextTrack);
            }
        }

        [RelayCommand]
        public void PlayPrevious()
        {
            var previousTrack = _appStateService.GetPreviousTrack();
            if (previousTrack != null)
            {
                PlayTrack(previousTrack);
            }
        }

        public void PlayTrack(AudioTrack track)
        {
            if (track == null) return;
            
            CurrentTrack = track;
            _appStateService.SetCurrentTrack(track);
            Play();
        }

        public void StartDragging()
        {
            _isUserDraggingSlider = true;
        }

        public void EndDragging()
        {
            _isUserDraggingSlider = false;
            _audioService.Seek(SliderPosition);
        }
    }
}