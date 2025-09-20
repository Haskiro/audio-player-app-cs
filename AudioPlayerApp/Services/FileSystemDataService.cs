using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using AudioPlayerApp.Models;
using Microsoft.Maui.Storage;

public class FileSystemDataService
{
    private readonly string _playlistsFilePath;
    
    public FileSystemDataService()
    {
        _playlistsFilePath = Path.Combine(FileSystem.AppDataDirectory, "playlists.json");
    }
    
    public async Task SavePlaylistsAsync(ObservableCollection<Playlist> playlists)
    {
        try
        {
            var json = JsonSerializer.Serialize(playlists);
            await File.WriteAllTextAsync(_playlistsFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving playlists: {ex.Message}");
        }
    }
    
    public async Task<ObservableCollection<Playlist>> LoadPlaylistsAsync()
    {
        try
        {
            if (File.Exists(_playlistsFilePath))
            {
                var json = await File.ReadAllTextAsync(_playlistsFilePath);
                return JsonSerializer.Deserialize<ObservableCollection<Playlist>>(json) 
                       ?? new ObservableCollection<Playlist>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading playlists: {ex.Message}");
        }
        
        return new ObservableCollection<Playlist>();
    }
}