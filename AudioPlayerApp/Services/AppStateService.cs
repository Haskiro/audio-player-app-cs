using System;
using AudioPlayerApp.Models;

public class AppStateService
{
    public Playlist CurrentPlaylist { get; private set; }
    public AudioTrack CurrentTrack { get; private set; }
    
    public event EventHandler<AudioTrack> CurrentTrackChanged;

    public void SetCurrentPlaylist(Playlist playlist)
    {
        CurrentPlaylist = playlist;
    }
    
    public void SetCurrentTrack(AudioTrack track)
    {
        CurrentTrack = track;
        CurrentTrackChanged?.Invoke(this, track);
    }

    public AudioTrack GetNextTrack()
    {
        if (CurrentPlaylist == null || CurrentTrack == null) 
            return null;

        var tracks = CurrentPlaylist.Tracks;
        var currentIndex = tracks.ToList().IndexOf(CurrentTrack);
        
        if (currentIndex < 0) 
            return null;
        
        var nextIndex = (currentIndex + 1) % tracks.Count;
        return tracks.ElementAtOrDefault(nextIndex);
    }

    public AudioTrack GetPreviousTrack()
    {
        if (CurrentPlaylist == null || CurrentTrack == null) 
            return null;

        var tracks = CurrentPlaylist.Tracks;
        var currentIndex = tracks.ToList().IndexOf(CurrentTrack);
        
        if (currentIndex < 0) 
            return null;
        
        var prevIndex = (currentIndex - 1 + tracks.Count) % tracks.Count;
        return tracks.ElementAtOrDefault(prevIndex);
    }
}