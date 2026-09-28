using System.Text.Json;
using System.Text.Json.Serialization;
using Signal.Bot.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents a synced story message from a linked device.
/// </summary>
public record SyncStoryMessage
{
    /// <summary>
    /// Gets or sets whether replies to this story are allowed.
    /// </summary>
    [JsonPropertyName("allowsReplies")]
    public bool? AllowsReplies { get; set; }

    /// <summary>
    /// Gets or sets the destination phone number.
    /// </summary>
    [JsonPropertyName("destinationNumber")]
    public string? DestinationNumber { get; set; }

    /// <summary>
    /// Gets or sets the destination UUID.
    /// </summary>
    [JsonPropertyName("destinationUuid")]
    public Guid DestinationId { get; set; }

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
