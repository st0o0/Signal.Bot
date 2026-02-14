using System.Text.Json.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents an emoji reaction to a message, including the target message information.
/// </summary>
public class ReactionData
{
    /// <summary>
    /// Gets or sets the emoji used for the reaction (e.g., "👍", "❤️", "😂").
    /// </summary>
    [JsonPropertyName("emoji")] 
    public string? Emoji { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this reaction should be removed. If <see langword="true"/>, removes the reaction; if <see langword="false"/> or <see langword="null"/>, adds it.
    /// </summary>
    [JsonPropertyName("remove")] 
    public bool? Remove { get; set; }

    /// <summary>
    /// Gets or sets the phone number or identifier of the author of the message being reacted to.
    /// </summary>
    [JsonPropertyName("targetAuthor")] 
    public string? TargetAuthor { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the target message was sent.
    /// </summary>
    [JsonPropertyName("targetSentTimestamp")] 
    public DateTime TargetSent { get; set; }
}