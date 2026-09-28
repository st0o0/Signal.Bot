using System.Text.Json;
using System.Text.Json.Serialization;
using Signal.Bot.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents a received unpin message event in a group.
/// </summary>
public record ReceivedUnpinMessage
{
    /// <summary>
    /// Gets or sets the author identifier of the unpinned message.
    /// </summary>
    [JsonPropertyName("targetAuthor")]
    public string? TargetAuthor { get; set; }

    /// <summary>
    /// Gets or sets the phone number of the unpinned message author.
    /// </summary>
    [JsonPropertyName("targetAuthorNumber")]
    public string? TargetAuthorNumber { get; set; }

    /// <summary>
    /// Gets or sets the UUID of the unpinned message author.
    /// </summary>
    [JsonPropertyName("targetAuthorUuid")]
    public Guid TargetAuthorId { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of the message being unpinned.
    /// </summary>
    [JsonPropertyName("targetSentTimestamp")]
    public DateTime TargetSent { get; set; }

    /// <inheritdoc />
    public override string ToString() => JsonSerializer.Serialize(this, JsonBotAPI.Get(GetType()));
}
