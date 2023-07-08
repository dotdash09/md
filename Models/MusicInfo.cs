using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace MusicDown.Models;

public class MusicInfo
{
    /// <summary>
    /// 앨범명
    /// </summary>
    [JsonPropertyName("album")]
    public string Album { get; set; }

    /// <summary>
    /// 노래명
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// 아티스트 문자열 (파일 기록에 사용하지 않음)
    /// </summary>
    [JsonPropertyName("artist")]
    public string Artist { get; set; }

    /// <summary>
    /// 아티스트 배열
    /// </summary>
    [JsonIgnore]
    public string[] Artists => Artist.Split(',', StringSplitOptions.TrimEntries);

    [JsonIgnore]
    public string[] AlbumArtists { get; set; }

    /// <summary>
    /// 출시년
    /// </summary>
    [JsonPropertyName("release_year")]
    public uint? ReleaseYear { get; set; }

    /// <summary>
    /// 앨범 트랙수
    /// </summary>
    [JsonPropertyName("n_entries")]
    public uint? Entries { get; set; }

    /// <summary>
    /// 트랙 번호
    /// </summary>
    [JsonPropertyName("playlist_index")]
    public uint? PlaylistIndex { get; set; }

    [JsonPropertyName("id")]
    public string SongId { get; set; }

    [JsonPropertyName("playlist_id")]
    public string AlbumId { get; set; }

    [JsonPropertyName("channel_id")]
    public string ChanneId { get; set; }

    /// <summary>
    /// 음악 포맷 (파일 기록에 사용하지 않음)
    /// </summary>
    [JsonPropertyName("formats")]
    public List<MusicInfoFormat> Formats { get; set; }

    /// <summary>
    /// 가장 고음질을 가져옴
    /// </summary>
    // [JsonIgnore]
    // public MusicInfoFormat Format => Formats.Where(n => n.FormatId == "251").FirstOrDefault();

    [JsonIgnore]
    public bool HasFormatOpus => Formats.Where(n => n.FormatId == "251").Any();

    [JsonIgnore]
    public bool HasM4AFormat => Formats.Where(n => n.FormatId == "141").Any();

    [JsonIgnore]
    public string JsonFilePath { get; set; }

    /// <summary>
    /// 썸내일 리스트 (파일 기록에 사용하지 않음)
    /// </summary>
    /// <value></value>
    [JsonPropertyName("thumbnails")]
    public List<MusicInfoThumbnails> Thumbnails { get; set; }

    /// <summary>
    /// 정사각형으로 된 가장 큰 썸내일을 가져옴.
    /// </summary>
    [JsonIgnore]
    public MusicInfoThumbnails Thumbnail
    {
        get
        {
            return Thumbnails.Where(n => n.Height != 0 || n.Width != 0)
                .Where(n => n.Height == n.Width)
                .MaxBy(n => n.Height);
        }
    }
}