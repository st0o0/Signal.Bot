using System.Text.Json;
using System.Text.Json.Serialization;
using Signal.Bot.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents an admin delete action on a message in a group.
/// </summary>
public record AdminDelete
{
    /// <summary>
    /// Gets or sets the author identifier of the target message.
    /// </summary>
    [JsonPropertyName("targetAuthor")]
    public string? TargetAuthor { get; set; }

    /// <summary>
    /// Gets or sets the phone number of the target message author.
    /// </summary>
    [JsonPropertyName("targetAuthorNumber")]
    public string? TargetAuthorNumber { get; set; }

    /// <summary>
    /// Gets or sets the UUID of the target message author.
    /// </summary>
    [JsonPropertyName("targetAuthorUuid")]
    public Guid TargetAuthorId { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of the message being deleted.
    /// </summary>
    [JsonPropertyName("targetSentTimestamp")]
    public DateTime TargetSent { get; set; }

    /// <inheritdoc />
    public override string ToString() => JsonSerializer.Serialize(this, JsonBotAPI.Get(GetType()));
}
