using System.Text.Json.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents a typing indicator message showing that a user is composing a message.
/// </summary>
public class TypingMessage
{
    /// <summary>
    /// Gets or sets the timestamp when the typing indicator was sent.
    /// </summary>
    [JsonPropertyName("timestamp")] 
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the action type (e.g., "started" for typing started, "stopped" for typing stopped).
    /// </summary>
    [JsonPropertyName("action")] 
    public string? Action { get; set; }
}