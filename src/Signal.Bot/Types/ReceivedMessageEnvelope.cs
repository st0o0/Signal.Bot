using System.Text.Json.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents a received message envelope container with the message content and account information.
/// </summary>
public class ReceivedMessageEnvelope
{
    /// <summary>
    /// Gets or sets the envelope containing the message metadata and content.
    /// </summary>
    /// <seealso cref="Envelope"/>
    [JsonPropertyName("envelope")] 
    public Envelope? Envelope { get; set; }

    /// <summary>
    /// Gets or sets the phone number of the Signal account that received the message.
    /// </summary>
    [JsonPropertyName("account")] 
    public string? Account { get; set; }
}