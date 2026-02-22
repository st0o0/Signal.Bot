using System.Text.Json;
using System.Text.Json.Serialization;
using Signal.Bot.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents a received message envelope container with the message content and account information.
/// </summary>
public record ReceivedMessage
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
    
    /// <inheritdoc />
    public override string ToString() => JsonSerializer.Serialize(this, JsonBotAPI.Get(GetType()));
}