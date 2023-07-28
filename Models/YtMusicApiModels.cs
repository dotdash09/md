using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace MusicDown.Models;

public class YtMusicApiChannel
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("channelId")]
    public string ChannelId { get; set; }

    [JsonPropertyName("albums")]
    public YtMusicApiAlbums Albums { get; set; }

    [JsonPropertyName("singles")]
    public YtMusicApiAlbums Singles { get; set; }
}

public class YtMusicApiAlbums
{
    [JsonPropertyName("browseId")]
    public string BrowseId { get; set; }

    [JsonPropertyName("params")]
    public string Params { get; set; }

    [JsonPropertyName("results")]
    public List<YtMusicApiAlbumResults> Results { get; set; }

    [JsonIgnore]
    public bool HasMoreAlbums => !string.IsNullOrEmpty(BrowseId) && !string.IsNullOrEmpty(Params);
}

public class YtMusicApiAlbumResults
{
    [JsonPropertyName("title")]
    public string Title { get; set; }

    // [JsonPropertyName("year")]
    [JsonIgnore]
    public string Year { get; set; }

    [JsonPropertyName("browseId")]
    public string BrowseId { get; set; }

    [JsonPropertyName("thumbnails")]
    public List<YtMusicApiThumbnails> Thumbnails { get; set; }

    [JsonIgnore]
    public YtMusicApiThumbnails Thumbnail =>
        Thumbnails.Where(n => n.Height == n.Width).MaxBy(n => n.Height);

    public YTMusicApiAlbumTrack AlbumTrack { get; set; }
}

public class YTMusicApiAlbumTrack
{
    private string _type;

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("year")]
    public string Year { get; set; }

    [JsonPropertyName("type")]
    public string Type
    {
        set => _type = value;
        get
        {
            if (_type == "앨범")
                return "Album";
            else if (_type == "싱글")
                return "Single";
            else
                return _type;
        }
    }

    [JsonPropertyName("tracks")]
    public List<YTMusicApiTrack> YTMusicApiTrack { get; set; }

    [JsonPropertyName("audioPlaylistId")]
    public string AudioPlaylistId { get; set; }
}

public class YTMusicApiTrack
{
    [JsonPropertyName("videoId")]
    public string VideoId { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("isAvailable")]
    public bool IsAvailable { get; set; }
}

public class YtMusicApiThumbnails
{
    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("width")]
    public int Width { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }
}