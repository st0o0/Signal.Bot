using System.Text.Json;
using System.Text.Json.Serialization;
using Signal.Bot.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents a voice or video call signaling message.
/// </summary>
public record CallMessage
{
    /// <summary>
    /// Gets or sets the hangup message if the call was ended.
    /// </summary>
    [JsonPropertyName("hangupMessage")]
    public HangupMessage? HangupMessage { get; set; }

    /// <summary>
    /// Gets or sets the offer message if a call is being initiated.
    /// </summary>
    [JsonPropertyName("offerMessage")]
    public OfferMessage? OfferMessage { get; set; }

    /// <summary>
    /// Gets or sets the ICE update messages for WebRTC connectivity negotiation.
    /// </summary>
    [JsonPropertyName("iceUpdateMessages")]
    public IceUpdateMessage[]? IceUpdateMessages { get; set; }

    /// <inheritdoc />
    public override string ToString() => JsonSerializer.Serialize(this, JsonBotAPI.Get(GetType()));
}
