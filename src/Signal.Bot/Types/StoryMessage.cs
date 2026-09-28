using System.Text.Json;
using System.Text.Json.Serialization;
using Signal.Bot.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents a story message with either a file or text attachment.
/// </summary>
public record StoryMessage
{
    /// <summary>
    /// Gets or sets whether replies to this story are allowed.
    /// </summary>
    [JsonPropertyName("allowsReplies")]
    public bool AllowsReplies { get; set; }

    /// <summary>
    /// Gets or sets the file attachment for this story.
    /// </summary>
    [JsonPropertyName("fileAttachment")]
    public Attachment? FileAttachment { get; set; }

    /// <summary>
    /// Gets or sets the group ID if this story was sent to a group.
    /// </summary>
    [JsonPropertyName("groupId")]
    public string? GroupId { get; set; }

    /// <summary>
    /// Gets or sets the text attachment for this story.
    /// </summary>
    [JsonPropertyName("textAttachment")]
    public TextAttachment? TextAttachment { get; set; }

    /// <inheritdoc />
    public override string ToString() => JsonSerializer.Serialize(this, JsonBotAPI.Get(GetType()));
}
