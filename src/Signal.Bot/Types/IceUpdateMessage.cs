using System.Text.Json.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents an ICE (Interactive Connectivity Establishment) update message used in WebRTC call signaling.
/// </summary>
public record IceUpdateMessage
{
    /// <summary>
    /// Gets or sets the ICE candidate identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public long? Id { get; set; }

    /// <summary>
    /// Gets or sets the opaque ICE candidate data.
    /// </summary>
    [JsonPropertyName("opaque")]
    public string? Opaque { get; set; }
}
