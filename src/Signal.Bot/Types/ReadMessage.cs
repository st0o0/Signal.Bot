using System.Text.Json.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents a read receipt for a message, indicating that a specific user has read the message.
/// </summary>
public class ReadMessage
{
    /// <summary>
    /// Gets or sets the identifier of the user who read the message.
    /// </summary>
    [JsonPropertyName("sender")] 
    public string? Sender { get; set; }

    /// <summary>
    /// Gets or sets the phone number of the user who read the message.
    /// </summary>
    [JsonPropertyName("senderNumber")] 
    public string? SenderNumber { get; set; }

    /// <summary>
    /// Gets or sets the UUID of the user who read the message.
    /// </summary>
    [JsonPropertyName("senderUuid")] 
    public Guid SenderUuid { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the message was read.
    /// </summary>
    [JsonPropertyName("timestamp")] 
    public DateTime Timestamp { get; set; }
}