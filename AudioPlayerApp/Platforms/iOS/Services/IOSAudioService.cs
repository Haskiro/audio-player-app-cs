using System;
using AVFoundation;
using CoreFoundation;
using CoreMedia;
using Foundation;
using AudioPlayerApp.Services;

namespace AudioPlayerApp.Platforms.iOS.Services;

public class IOSAudioService : IAudioService, IDisposable
{
    private AVPlayer _player;
    private AVPlayerItem _currentItem;
    private IDisposable _timeObserver;
    private bool _isDisposed;

    public event EventHandler<double> PositionChanged;
    public event EventHandler<bool> PlaybackStateChanged;
    public event EventHandler PlaybackCompleted;

    public IOSAudioService()
    {
        _player = new AVPlayer();
        SetupNotifications();
    }

    public void Play(string filePath)
    {
        if (_player.CurrentItem != null && _player.CurrentItem.Asset is AVUrlAsset urlAsset &&
            urlAsset.Url.Path == filePath)
        {
            _player.Play();
            PlaybackStateChanged?.Invoke(this, true);
            return;
        }

        Stop();

        var url = NSUrl.FromFilename(filePath);
        _currentItem = AVPlayerItem.FromUrl(url);
        _player.ReplaceCurrentItemWithPlayerItem(_currentItem);

        _player.Play();
        PlaybackStateChanged?.Invoke(this, true);

        RemoveTimeObserver();
        _timeObserver = _player.AddPeriodicTimeObserver(CMTime.FromSeconds(0.5, 1), DispatchQueue.MainQueue, time =>
        {
            var position = time.Seconds;
            PositionChanged?.Invoke(this, position);

            if (position >= _currentItem.Duration.Seconds)
            {
                PlaybackCompleted?.Invoke(this, EventArgs.Empty);
            }
        });
    }

    public void Pause()
    {
        _player.Pause();
        PlaybackStateChanged?.Invoke(this, false);
    }

    public void Stop()
    {
        _player.Pause();
        _player.Seek(CMTime.FromSeconds(0, 1));
        PlaybackStateChanged?.Invoke(this, false);
    }

    public void Seek(double position)
    {
        _player.Seek(CMTime.FromSeconds(position, 1));
    }

    public void SetSpeed(float speed)
    {
        _player.Rate = speed;
    }

    public double GetDuration() => _player.CurrentItem?.Duration.Seconds ?? 0;

    public double GetPosition() => _player.CurrentTime.Seconds;

    public bool IsPlaying => _player.Rate > 0;

    private void SetupNotifications()
    {
        NSNotificationCenter.DefaultCenter.AddObserver(
            AVPlayerItem.DidPlayToEndTimeNotification,
            notification => PlaybackCompleted?.Invoke(this, EventArgs.Empty)
        );
    }

    private void RemoveTimeObserver()
    {
        _timeObserver?.Dispose();
        _timeObserver = null;
    }

    public void Dispose()
    {
        if (_isDisposed) return;

        RemoveTimeObserver();
        _player?.Dispose();
        _currentItem?.Dispose();

        _isDisposed = true;
    }
}
