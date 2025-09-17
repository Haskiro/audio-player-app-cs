using System;

namespace AudioPlayerApp.Models;

public class AudioTrack
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string FilePath { get; set; }
    public string Title { get; set; }
    public string Artist { get; set; }
    public string Album { get; set; }
    public TimeSpan Duration { get; set; }
    public string FormattedDuration => Duration.ToString(@"mm\:ss");
}