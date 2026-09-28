using System.Text.Json;
using System.Text.Json.Serialization;
using Signal.Bot.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents a copy of a message sent by the local user, synchronized across linked devices.
/// </summary>
public record SentMessage
{
    /// <summary>
    /// Gets or sets the destination identifier of the message recipient.
    /// </summary>
    [JsonPropertyName("destination")]
    public string? Destination { get; set; }

    /// <summary>
    /// Gets or sets the phone number of the message recipient.
    /// </summary>
    [JsonPropertyName("destinationNumber")]
    public string? DestinationNumber { get; set; }

    /// <summary>
    /// Gets or sets the UUID of the message recipient.
    /// </summary>
    [JsonPropertyName("destinationUuid")]
    public Guid? DestinationId { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the message was sent.
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTime? Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the text content of the sent message.
    /// </summary>
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    /// <summary>
    /// Gets or sets the duration until the message expires and is deleted.
    /// </summary>
    [JsonPropertyName("expiresInSeconds")]
    public TimeSpan? ExpiresIn { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this message is an update to the disappearing message timer setting.
    /// </summary>
    [JsonPropertyName("isExpirationUpdate")]
    public bool? IsExpirationUpdate { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this message can only be viewed once before disappearing.
    /// </summary>
    [JsonPropertyName("viewOnce")]
    public bool? ViewOnce { get; set; }

    /// <summary>
    /// Gets or sets the reaction data if this message is a reaction to another message.
    /// </summary>
    [JsonPropertyName("reaction")]
    public Reaction? Reaction { get; set; }

    /// <summary>
    /// Gets or sets the list of file attachments included with the message.
    /// </summary>
    [JsonPropertyName("attachments")]
    public List<Attachment>? Attachments { get; set; }

    /// <summary>
    /// Gets or sets the group information if this message was sent to a group.
    /// </summary>
    [JsonPropertyName("groupInfo")]
    public GroupInfo? GroupInfo { get; set; }

    /// <summary>
    /// Gets or sets the list of link preview data for URLs in the message.
    /// </summary>
    [JsonPropertyName("previews")]
    public List<Preview>? Previews { get; set; }

    /// <summary>
    /// Gets or sets the remote delete data if this message deletes a previously sent message.
    /// </summary>
    [JsonPropertyName("remoteDelete")]
    public Acknowledged? RemoteDelete { get; set; }

    /// <inheritdoc />
    public override string ToString() => JsonSerializer.Serialize(this, JsonBotAPI.Get(GetType()));
}
