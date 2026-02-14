using System.Text.Json.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents an acknowledgment response from the Signal Bot API, indicating that an operation was successfully received and processed.
/// </summary>
public class Acknowledged
{
    /// <summary>
    /// Gets or sets the timestamp when the operation was acknowledged by the Signal server.
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }
}