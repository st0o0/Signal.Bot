using System.Text.Json.Serialization;

namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to remove an emoji reaction from a specific message.
/// </summary>
/// <param name="Number">The phone number of the Signal account removing the reaction.</param>
public record RemoveReactionRequest(string Number) : RequestBase<string>($"v1/reactions/{Number}", HttpMethod.Delete)
{
    /// <summary>
    /// Gets or sets the emoji reaction to remove (e.g., "👍", "❤️", "😂").
    /// </summary>
    [JsonPropertyName("reaction")] 
    public string? Reaction { get; set; }

    /// <summary>
    /// Gets or sets the phone number or group ID of the conversation containing the message.
    /// </summary>
    [JsonPropertyName("recipient")] 
    public string? Recipient { get; set; }

    /// <summary>
    /// Gets or sets the phone number of the author of the message that was reacted to.
    /// </summary>
    [JsonPropertyName("target_author")] 
    public string? TargetAuthor { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of the message that was reacted to. Defaults to current UTC time.
    /// </summary>
    [JsonPropertyName("timestamp")] 
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}