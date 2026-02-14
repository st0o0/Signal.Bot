using System.Text.Json.Serialization;
using Signal.Bot.Types;

namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to send a message via the Signal Bot API.
/// </summary>
public record SendMessageRequest() : RequestBase<Acknowledged>("v2/send")
{
    /// <summary>
    /// Gets or sets the array of base64-encoded attachments to send with the message.
    /// </summary>
    [JsonPropertyName("base64_attachments")]
    public string[]? Attachments { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of the message to edit. Used when editing an existing message.
    /// </summary>
    [JsonPropertyName("edit_timestamp")]
    public DateTime EditTimestamp { get; set; }

    /// <summary>
    /// Gets or sets the link preview configuration for URLs in the message.
    /// </summary>
    /// <seealso cref="LinkPreview"/>
    [JsonPropertyName("link_preview")]
    public LinkPreview? LinkPreview { get; set; }

    /// <summary>
    /// Gets or sets the array of mentions in the message text.
    /// </summary>
    /// <seealso cref="MessageMention"/>
    [JsonPropertyName("mentions")]
    public MessageMention[]? Mentions { get; set; }

    /// <summary>
    /// Gets or sets the text content of the message.
    /// </summary>
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    /// <summary>
    /// Gets or sets whether the sender should receive a notification for their own message.
    /// </summary>
    [JsonPropertyName("notify_self")]
    public bool? NotifySelf { get; set; }

    /// <summary>
    /// Gets or sets the sender's phone number in international format.
    /// </summary>
    [JsonPropertyName("number")]
    public string? Number { get; set; }

    /// <summary>
    /// Gets or sets the author of the quoted message when replying.
    /// </summary>
    [JsonPropertyName("quote_author")]
    public string? QuoteAuthor { get; set; }

    /// <summary>
    /// Gets or sets the array of mentions in the quoted message.
    /// </summary>
    /// <seealso cref="MessageMention"/>
    [JsonPropertyName("quote_mentions")]
    public MessageMention[]? QuoteMentions { get; set; }

    /// <summary>
    /// Gets or sets the text content of the message being quoted or replied to.
    /// </summary>
    [JsonPropertyName("quote_message")]
    public string? QuoteMessage { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of the message being quoted or replied to.
    /// </summary>
    [JsonPropertyName("quote_timestamp")]
    public DateTime QuoteTimestamp { get; set; }

    /// <summary>
    /// Gets or sets the array of recipient phone numbers or group IDs.
    /// </summary>
    [JsonPropertyName("recipients")]
    public string[]? Recipients { get; set; }

    /// <summary>
    /// Gets or sets the sticker identifier or base64-encoded sticker data to send.
    /// </summary>
    [JsonPropertyName("sticker")]
    public string? Sticker { get; set; }

    /// <summary>
    /// Gets or sets the text formatting mode for the message.
    /// </summary>
    /// <seealso cref="TextMode"/>
    [JsonPropertyName("text_mode")]
    public TextMode? TextMode { get; set; }

    /// <summary>
    /// Gets or sets whether the message should be viewable only once before disappearing.
    /// </summary>
    [JsonPropertyName("view_once")]
    public bool? ViewOnce { get; set; }
}

/// <summary>
/// Represents a mention of a user within a message, specifying the author and the text position.
/// </summary>
public record MessageMention
{
    /// <summary>
    /// Gets or sets the phone number or identifier of the user being mentioned.
    /// </summary>
    [JsonPropertyName("author")]
    public string? Author { get; set; }

    /// <summary>
    /// Gets or sets the length of the mention text in characters.
    /// </summary>
    [JsonPropertyName("length")]
    public int? Length { get; set; }

    /// <summary>
    /// Gets or sets the zero-based starting character position of the mention in the message text.
    /// </summary>
    [JsonPropertyName("start")]
    public int? Start { get; set; }
}

/// <summary>
/// Represents a link preview configuration for URLs in a message, including title, description, and thumbnail.
/// </summary>
public record LinkPreview
{
    /// <summary>
    /// Gets or sets the base64-encoded thumbnail image for the link preview.
    /// </summary>
    /// <seealso cref="Base64String"/>
    [JsonPropertyName("base64_thumbnail")]
    public string? Thumbnail { get; set; }

    /// <summary>
    /// Gets or sets the description text for the link preview.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the title text for the link preview.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the URL to be previewed.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

/// <summary>
/// Defines the text formatting modes available for Signal messages.
/// </summary>
public enum TextMode
{
    /// <summary>
    /// Plain text mode without any formatting.
    /// </summary>
    [JsonStringEnumMemberName("normal")] Normal = 0,

    /// <summary>
    /// Styled text mode with support for bold, italic, and other formatting.
    /// </summary>
    [JsonStringEnumMemberName("styled")] Styled = 1,
}