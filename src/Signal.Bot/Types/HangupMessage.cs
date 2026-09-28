using System.Text.Json.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents a call hangup signaling message.
/// </summary>
public record HangupMessage
{
    /// <summary>
    /// Gets or sets the call identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public long? Id { get; set; }

    /// <summary>
    /// Gets or sets the hangup type indicating how the call ended.
    /// </summary>
    [JsonPropertyName("type")]
    public HangupType? Type { get; set; }

    /// <summary>
    /// Gets or sets the device identifier of the device that hung up.
    /// </summary>
    [JsonPropertyName("deviceId")]
    public long? DeviceId { get; set; }
}
