using System.Text.Json.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents a WebRTC call offer message used to initiate a voice or video call.
/// </summary>
public record OfferMessage
{
    /// <summary>
    /// Gets or sets the call offer identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public long? Id { get; set; }

    /// <summary>
    /// Gets or sets the opaque call offer data.
    /// </summary>
    [JsonPropertyName("opaque")]
    public string? Opaque { get; set; }

    /// <summary>
    /// Gets or sets the type of call being offered.
    /// </summary>
    [JsonPropertyName("type")]
    public CallType Type { get; set; }
}
