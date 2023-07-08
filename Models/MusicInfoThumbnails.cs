using System.Text.Json.Serialization;

namespace MusicDown.Models;

/// <summary>
/// 썸내일
/// </summary>
public class MusicInfoThumbnails
{
    /// <summary>
    /// 주소
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; }
    
    /// <summary>
    /// 높이
    /// </summary>
    [JsonPropertyName("height")]
    public int Height { get; set; }

    /// <summary>
    /// 넓이
    /// </summary>
    [JsonPropertyName("width")]
    public int Width { get; set; }

}