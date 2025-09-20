using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AudioPlayerApp.Models;

public class Playlist : INotifyPropertyChanged
{
    private string _name;
    private DateTime _createdDate;
    private DateTime _modifiedDate;
    private ObservableCollection<AudioTrack> _tracks = new ObservableCollection<AudioTrack>();

    public string Name
    {
        get => _name;
        set
        {
            if (_name != value)
            {
                _name = value;
                OnPropertyChanged();
            }
        }
    }

    public DateTime CreatedDate
    {
        get => _createdDate;
        set
        {
            if (_createdDate != value)
            {
                _createdDate = value;
                OnPropertyChanged();
            }
        }
    }

    public DateTime ModifiedDate
    {
        get => _modifiedDate;
        set
        {
            if (_modifiedDate != value)
            {
                _modifiedDate = value;
                OnPropertyChanged();
            }
        }
    }

    public ObservableCollection<AudioTrack> Tracks
    {
        get => _tracks;
        set
        {
            if (_tracks != value)
            {
                _tracks = value;
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