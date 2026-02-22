using System.Text.Json.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// 
/// </summary>
public record HangupMessage
{
    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("id")]
    public long? Id { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("type")]
    public HangupType? Type { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("deviceId")]
    public long? DeviceId { get; set; }
}