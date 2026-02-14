using System.Text.Json.Serialization;

namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to add an emoji reaction to a specific message.
/// </summary>
/// <param name="Number">The phone number of the Signal account sending the reaction.</param>
public record AddReactionRequest(string Number) : RequestBase($"v1/reactions/{Number}")
{
    /// <summary>
    /// Gets or sets the emoji to react with (e.g., "👍", "❤️", "😂").
    /// </summary>
    [JsonPropertyName("reaction")] 
    public string? Reaction { get; set; }

    /// <summary>
    /// Gets or sets the phone number or group ID of the conversation containing the message.
    /// </summary>
    [JsonPropertyName("recipient")] 
    public string? Recipient { get; set; }

    /// <summary>
    /// Gets or sets the phone number of the author of the message being reacted to.
    /// </summary>
    [JsonPropertyName("target_author")] 
    public string? TargetAuthor { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of the message being reacted to.
    /// </summary>
    [JsonPropertyName("timestamp")] 
    public DateTime Timestamp { get; set; }
}