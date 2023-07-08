using System.Text.Json.Serialization;

namespace MusicDown.Models;

/// <summary>
/// 음악 포멧
/// </summary>
public class MusicInfoFormat
{
    /// <summary>
    /// 포맷 아이디 (opus : 251) 
    /// </summary>
    /// <value></value>
    [JsonPropertyName("format_id")]
    public string FormatId { get; set; }

    /// <summary>
    /// 음악 주소
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; }
}