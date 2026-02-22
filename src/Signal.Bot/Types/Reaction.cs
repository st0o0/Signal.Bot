using System.Text.Json;
using System.Text.Json.Serialization;
using Signal.Bot.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents an emoji reaction to a message, including the target message information.
/// </summary>
public record Reaction
{
    /// <summary>
    /// Gets or sets the emoji used for the reaction (e.g., "👍", "❤️", "😂").
    /// </summary>
    [JsonPropertyName("emoji")]
    public string? Emoji { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this reaction should be removed. If <see langword="true"/>, removes the reaction; if <see langword="false"/> or <see langword="null"/>, adds it.
    /// </summary>
    [JsonPropertyName("isRemove")]
    public bool? IsRemove { get; set; }

    /// <summary>
    /// Gets or sets the phone number or identifier of the author of the message being reacted to.
    /// </summary>
    [JsonPropertyName("targetAuthor")]
    public string? TargetAuthor { get; set; }

    /// <summary>
    /// TBD
    /// </summary>
    [JsonPropertyName("targetAuthorNumber")]
    public string? TargetAuthorNumber { get; set; }

    /// <summary>
    /// TBD
    /// </summary>
    [JsonPropertyName("targetAuthorUuid")]
    public Guid TargetAuthorUuid { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the target message was sent.
    /// </summary>
    [JsonPropertyName("targetSentTimestamp")]
    public DateTime TargetSent { get; set; }
    
    /// <inheritdoc />
    public override string ToString() => JsonSerializer.Serialize(this, JsonBotAPI.Get(GetType()));
}