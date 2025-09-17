using System;
using System.Collections.ObjectModel;

namespace AudioPlayerApp.Models;

public class Playlist
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; }
    public ObservableCollection<AudioTrack> Tracks { get; set; } = new();
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime ModifiedDate { get; set; } = DateTime.Now;
}