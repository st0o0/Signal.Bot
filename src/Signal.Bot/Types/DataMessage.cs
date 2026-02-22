using System.Text.Json;
using System.Text.Json.Serialization;
using Signal.Bot.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents the main content of a Signal message, including text, attachments, reactions, and metadata.
/// </summary>
public record DataMessage
{
    /// <summary>
    /// Gets or sets the timestamp when the message was sent.
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the text content of the message.
    /// </summary>
    [JsonPropertyName("message")]
    public string? Message { get; set; }
    
    /// <summary>
    /// Gets or sets the list of file attachments included with the message.
    /// </summary>
    [JsonPropertyName("attachments")]
    public List<Attachment>? Attachments { get; set; }

    /// <summary>
    /// Gets or sets the reaction data if this message is a reaction to another message.
    /// </summary>
    [JsonPropertyName("reaction")]
    public Reaction? Reaction { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this message can only be viewed once before disappearing.
    /// </summary>
    [JsonPropertyName("viewOnce")]
    public bool? ViewOnce { get; set; }

    /// <summary>
    /// Gets or sets the number of seconds until the message expires and is deleted.
    /// </summary>
    [JsonPropertyName("expiresInSeconds")]
    public TimeSpan? ExpiresIn { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this message is an update to the disappearing message timer setting.
    /// </summary>
    [JsonPropertyName("isExpirationUpdate")]
    public bool? IsExpirationUpdate { get; set; }

    /// <summary>
    /// Gets or sets the group information if this message was sent to a Signal v2 group.
    /// </summary>
    [JsonPropertyName("groupV2")]
    public GroupInfo? GroupV2 { get; set; }

    /// <summary>
    /// Gets or sets the list of user mentions in the message text.
    /// </summary>
    [JsonPropertyName("mentions")]
    public List<Mention>? Mentions { get; set; }

    /// <summary>
    /// Gets or sets the quoted message data if this message is a reply to another message.
    /// </summary>
    [JsonPropertyName("quote")]
    public QuoteData? Quote { get; set; }

    /// <summary>
    /// Gets or sets the list of read receipts for messages.
    /// </summary>
    [JsonPropertyName("readMessages")]
    public List<ReadMessage>? ReadMessages { get; set; }

    /// <summary>
    /// Gets or sets the list of link preview data for URLs in the message.
    /// </summary>
    [JsonPropertyName("previews")]
    public List<PreviewData>? Previews { get; set; }
    
    /// <inheritdoc />
    public override string ToString() => JsonSerializer.Serialize(this, JsonBotAPI.Get(GetType()));
}