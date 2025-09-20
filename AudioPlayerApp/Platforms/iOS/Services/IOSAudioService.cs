using System;
using AVFoundation;
using CoreMedia;
using Foundation;
using CoreFoundation;
using Microsoft.Maui.ApplicationModel;

namespace AudioPlayerApp.Platforms.iOS.Services
{
    public class IOSAudioService : IAudioService, IDisposable
    {
        private readonly AVPlayer _player;
        private NSObject _timeObserver;
        private AVPlayerItem _currentItem;

        public event EventHandler<double> PositionChanged;
        public event EventHandler PlaybackCompleted;
        public event EventHandler<bool> PlaybackStateChanged;
        public event EventHandler<double> DurationAvailable;

        public IOSAudioService()
        {
            _player = new AVPlayer
            {
                ActionAtItemEnd = AVPlayerActionAtItemEnd.Pause
            };
        }

        public void Play(string filePath)
        {
            try
            {
                Stop();
                
                var url = NSUrl.FromFilename(filePath);
                _currentItem = AVPlayerItem.FromUrl(url);
                _player.ReplaceCurrentItemWithPlayerItem(_currentItem);

                // Загрузка длительности
                var asset = _currentItem.Asset;
                asset.LoadValuesTaskAsync(new[] { "duration" }).ContinueWith(t =>
                {
                    var status = asset.StatusOfValue("duration", out NSError error);
                    if (status == AVKeyValueStatus.Loaded)
                    {
                        var dur = asset.Duration.Seconds;
                        if (dur > 0)
                        {
                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                DurationAvailable?.Invoke(this, dur);
                            });
                        }
                    }
                });

                _player.Play();
                PlaybackStateChanged?.Invoke(this, true);

                // Таймер позиционирования
                RemoveTimeObserver();
                var interval = CMTime.FromSeconds(0.5, 1);
                _timeObserver = _player.AddPeriodicTimeObserver(interval, DispatchQueue.MainQueue, time =>
                {
                    var position = time.Seconds;
                    PositionChanged?.Invoke(this, position);

                    var durationSeconds = _currentItem?.Duration.Seconds ?? 0;
                    if (durationSeconds > 0 && position >= durationSeconds - 0.5)
                    {
                        PlaybackCompleted?.Invoke(this, EventArgs.Empty);
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"IOSAudioService.Play error: {ex}");
            }
        }

        public void Resume()
        {
            _player.Play();
            PlaybackStateChanged?.Invoke(this, true);
        }

        public void Pause()
        {
            _player.Pause();
            PlaybackStateChanged?.Invoke(this, false);
        }

        public void Stop()
        {
            _player.Pause();
            _player.Seek(CoreMedia.CMTime.Zero);
            PlaybackStateChanged?.Invoke(this, false);
            RemoveTimeObserver();
        }

        public void Seek(double seconds)
        {
            var cmTime = CMTime.FromSeconds(seconds, 1);
            _player.Seek(cmTime);
        }

        public double GetCurrentPosition()
        {
            return _player.CurrentTime.Seconds;
        }

        public double GetDuration()
        {
            return _currentItem?.Duration.Seconds ?? 0;
        }

        public bool IsPlaying()
        {
            return _player.TimeControlStatus == AVPlayerTimeControlStatus.Playing;
        }
        
        public double GetTrackDuration(string filePath)
        {
            try
            {
                var url = NSUrl.FromFilename(filePath);
                var asset = AVAsset.FromUrl(url);

                // Синхронная загрузка длительности
                var task = asset.LoadValuesTaskAsync(new[] { "duration" });
                task.Wait();

                var status = asset.StatusOfValue("duration", out NSError error);
                if (status == AVKeyValueStatus.Loaded)
                {
                    var duration = asset.Duration.Seconds;
                    return duration > 0 ? duration : 0;
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetTrackDuration error: {ex.Message}");
                return 0;
            }
        }

        public void SetSpeed(double speed)
        {
            _player.Rate = (float)speed;
        }

        private void RemoveTimeObserver()
        {
            if (_timeObserver != null)
            {
                _player.RemoveTimeObserver(_timeObserver);
                _timeObserver.Dispose();
                _timeObserver = null;
            }
        }

        public void Dispose()
        {
            Stop();
            _player.Dispose();
            _currentItem?.Dispose();
        }
    }
}


public interface IAudioService
{
    void Play(string filePath);
    void Pause();
    void Resume();
    void Stop();
    void Seek(double seconds);
    double GetCurrentPosition();
    double GetTrackDuration(string filePath);
    void SetSpeed(double speed);

    event EventHandler<double> PositionChanged;
    event EventHandler PlaybackCompleted;
    event EventHandler<bool> PlaybackStateChanged;
    event EventHandler<double> DurationAvailable;
}