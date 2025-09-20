using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AudioPlayerApp.Models;

public class AudioTrack : INotifyPropertyChanged
{
    private string _title;
    private string _album;
    private string _filePath;
    private TimeSpan _duration;

    public string Title
    {
        get => _title;
        set
        {
            if (_title != value)
            {
                _title = value;
                OnPropertyChanged();
            }
        }
    }

    public string Album
    {
        get => _album;
        set
        {
            if (_album != value)
            {
                _album = value;
                OnPropertyChanged();
            }
        }
    }

    public string FilePath
    {
        get => _filePath;
        set
        {
            if (_filePath != value)
            {
                _filePath = value;
                OnPropertyChanged();
            }
        }
    }

    public TimeSpan Duration
    {
        get => _duration;
        set
        {
            if (_duration != value)
            {
                _duration = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}