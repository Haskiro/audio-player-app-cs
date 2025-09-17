using System;
using AudioPlayerApp.Models;
using AudioPlayerApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AudioPlayerApp.ViewModels;

public partial class PlayerViewModel : ObservableObject
{
    private readonly IAudioService _audioService;
    private Playlist _currentPlaylist;
    private AudioTrack _currentTrack;
    private int _currentTrackIndex;
    private double _duration;
    private bool _isSeeking;

    [ObservableProperty]
    private double _position;

    [ObservableProperty]
    private bool _isPlaying;

    [ObservableProperty]
    private float _playbackSpeed = 1.0f;

    [ObservableProperty]
    private string _currentTimeFormatted = "00:00";

    [ObservableProperty]
    private string _totalTimeFormatted = "00:00";

    public PlayerViewModel(IAudioService audioService)
    {
        _audioService = audioService;
        
        _audioService.PositionChanged += OnPositionChanged;
        _audioService.PlaybackStateChanged += OnPlaybackStateChanged;
        _audioService.PlaybackCompleted += OnPlaybackCompleted;
    }

    public AudioTrack CurrentTrack
    {
        get => _currentTrack;
        set
        {
            if (SetProperty(ref _currentTrack, value) && value != null)
            {
                PlayTrack(value);
            }
        }
    }

    public Playlist CurrentPlaylist
    {
        get => _currentPlaylist;
        set
        {
            if (SetProperty(ref _currentPlaylist, value) && value?.Tracks.Count > 0)
            {
                CurrentTrack = value.Tracks[0];
                _currentTrackIndex = 0;
            }
        }
    }

    private void OnPositionChanged(object sender, double position)
    {
        if (!_isSeeking)
        {
            Position = position;
            UpdateTimeFormatted();
        }
    }

    private void OnPlaybackStateChanged(object sender, bool isPlaying)
    {
        IsPlaying = isPlaying;
    }

    private void OnPlaybackCompleted(object sender, EventArgs e)
    {
        PlayNext();
    }

    private void PlayTrack(AudioTrack track)
    {
        _audioService.Stop();
        _audioService.Play(track.FilePath);
        _duration = _audioService.GetDuration();
        TotalTimeFormatted = FormatTime(_duration);
        IsPlaying = true;
    }

    private void UpdateTimeFormatted()
    {
        CurrentTimeFormatted = FormatTime(Position);
    }

    private string FormatTime(double seconds)
    {
        var timeSpan = TimeSpan.FromSeconds(seconds);
        return $"{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
    }

    [RelayCommand]
    private void PlayPause()
    {
        if (CurrentTrack == null) return;

        if (IsPlaying)
        {
            _audioService.Pause();
        }
        else
        {
            _audioService.Play(CurrentTrack.FilePath);
        }
    }

    [RelayCommand]
    private void Stop()
    {
        _audioService.Stop();
        Position = 0;
        UpdateTimeFormatted();
    }

    [RelayCommand]
    private void SeekForward()
    {
        var newPosition = Position + 10;
        if (newPosition > _duration) newPosition = _duration;
        
        _audioService.Seek(newPosition);
        Position = newPosition;
        UpdateTimeFormatted();
    }

    [RelayCommand]
    private void SeekBackward()
    {
        var newPosition = Position - 10;
        if (newPosition < 0) newPosition = 0;
        
        _audioService.Seek(newPosition);
        Position = newPosition;
        UpdateTimeFormatted();
    }

    [RelayCommand]
    private void ChangeSpeed()
    {
        var speeds = new[] { 0.5f, 0.75f, 1.0f, 1.25f, 1.5f, 2.0f };
        var currentIndex = Array.IndexOf(speeds, PlaybackSpeed);
        var nextIndex = (currentIndex + 1) % speeds.Length;
        
        PlaybackSpeed = speeds[nextIndex];
        _audioService.SetSpeed(PlaybackSpeed);
    }

    [RelayCommand]
    private void PlayNext()
    {
        if (CurrentPlaylist?.Tracks.Count > 0)
        {
            _currentTrackIndex = (_currentTrackIndex + 1) % CurrentPlaylist.Tracks.Count;
            CurrentTrack = CurrentPlaylist.Tracks[_currentTrackIndex];
        }
    }

    [RelayCommand]
    private void PlayPrevious()
    {
        if (CurrentPlaylist?.Tracks.Count > 0)
        {
            _currentTrackIndex = (_currentTrackIndex - 1 + CurrentPlaylist.Tracks.Count) % CurrentPlaylist.Tracks.Count;
            CurrentTrack = CurrentPlaylist.Tracks[_currentTrackIndex];
        }
    }

    public void OnSliderDragStarted()
    {
        _isSeeking = true;
    }

    public void OnSliderDragCompleted()
    {
        _isSeeking = false;
        _audioService.Seek(Position);
    }

    public void Dispose()
    {
        _audioService.PositionChanged -= OnPositionChanged;
        _audioService.PlaybackStateChanged -= OnPlaybackStateChanged;
        _audioService.PlaybackCompleted -= OnPlaybackCompleted;
        _audioService.Stop();
    }
}