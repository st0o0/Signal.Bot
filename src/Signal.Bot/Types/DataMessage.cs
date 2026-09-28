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
    [JsonPropertyName("groupInfo")]
    public GroupInfo? GroupInfo { get; set; }

    /// <summary>
    /// Gets or sets the list of user mentions in the message text.
    /// </summary>
    [JsonPropertyName("mentions")]
    public List<Mention>? Mentions { get; set; }

    /// <summary>
    /// Gets or sets the quoted message data if this message is a reply to another message.
    /// </summary>
    [JsonPropertyName("quote")]
    public Quote? Quote { get; set; }

    /// <summary>
    /// Gets or sets the list of read receipts for messages.
    /// </summary>
    [JsonPropertyName("readMessages")]
    public List<ReadMessage>? ReadMessages { get; set; }

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

    /// <summary>
    /// Gets or sets the admin delete data if an admin deleted a message in a group.
    /// </summary>
    [JsonPropertyName("adminDelete")]
    public AdminDelete? AdminDelete { get; set; }

    /// <summary>
    /// Gets or sets the list of shared contacts included in this message.
    /// </summary>
    [JsonPropertyName("contacts")]
    public List<SharedContact>? Contacts { get; set; }

    /// <summary>
    /// Gets or sets the group call update event data.
    /// </summary>
    [JsonPropertyName("groupCallUpdate")]
    public GroupCallUpdate? GroupCallUpdate { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this message includes a profile key.
    /// </summary>
    [JsonPropertyName("hasProfileKey")]
    public bool? HasProfileKey { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this message signals the end of a session.
    /// </summary>
    [JsonPropertyName("isEndSession")]
    public bool? IsEndSession { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this message is a profile key update.
    /// </summary>
    [JsonPropertyName("isProfileKeyUpdate")]
    public bool? IsProfileKeyUpdate { get; set; }

    /// <summary>
    /// Gets or sets the payment data if this message contains a payment.
    /// </summary>
    [JsonPropertyName("payment")]
    public Payment? Payment { get; set; }

    /// <summary>
    /// Gets or sets the pin message event if a message was pinned in a group.
    /// </summary>
    [JsonPropertyName("pinMessage")]
    public ReceivedPinMessage? PinMessage { get; set; }

    /// <summary>
    /// Gets or sets the poll creation data if this message creates a new poll.
    /// </summary>
    [JsonPropertyName("pollCreate")]
    public PollCreate? PollCreate { get; set; }

    /// <summary>
    /// Gets or sets the poll termination data if this message closes a poll.
    /// </summary>
    [JsonPropertyName("pollTerminate")]
    public PollTerminate? PollTerminate { get; set; }

    /// <summary>
    /// Gets or sets the poll vote data if this message contains a vote on a poll.
    /// </summary>
    [JsonPropertyName("pollVote")]
    public PollVote? PollVote { get; set; }

    /// <summary>
    /// Gets or sets the sticker data if this message contains a sticker.
    /// </summary>
    [JsonPropertyName("sticker")]
    public Sticker? Sticker { get; set; }

    /// <summary>
    /// Gets or sets the story context if this message is a reply to a story.
    /// </summary>
    [JsonPropertyName("storyContext")]
    public StoryContext? StoryContext { get; set; }

    /// <summary>
    /// Gets or sets the list of text formatting styles applied to the message text.
    /// </summary>
    [JsonPropertyName("textStyles")]
    public List<TextStyle>? TextStyles { get; set; }

    /// <summary>
    /// Gets or sets the unpin message event if a message was unpinned in a group.
    /// </summary>
    [JsonPropertyName("unpinMessage")]
    public ReceivedUnpinMessage? UnpinMessage { get; set; }

    /// <inheritdoc />
    public override string ToString() => JsonSerializer.Serialize(this, JsonBotAPI.Get(GetType()));
}