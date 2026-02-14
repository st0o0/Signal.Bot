using Signal.Bot.Requests;

namespace Signal.Bot;

/// <summary>
/// Fluent builder for constructing SendMessageRequest objects with various message properties and configurations.
/// </summary>
public class SendMessageRequestBuilder
{
    private SendMessageRequest _request = new();

    /// <summary>
    /// Creates a new instance of the SendMessageRequestBuilder.
    /// </summary>
    /// <returns>A new SendMessageRequestBuilder instance.</returns>
    public static SendMessageRequestBuilder Create() => new();

    /// <summary>
    /// Adds an attachment from a file path, automatically encoding it as base64 with the appropriate MIME type.
    /// </summary>
    /// <param name="filePath">The path to the file to attach.</param>
    /// <param name="includeFilename">If true, includes the filename in the data URI.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public SendMessageRequestBuilder WithAttachmentFromFile(string filePath, bool includeFilename = false)
    {
        return WithAttachment(Base64Attachment.FromFile(filePath, includeFilename: includeFilename));
    }

    /// <summary>
    /// Adds an attachment from a byte array with the specified MIME type and optional filename.
    /// </summary>
    /// <param name="bytes">The byte array containing the attachment data.</param>
    /// <param name="mimeType">The MIME type of the attachment (e.g., "image/png").</param>
    /// <param name="filename">Optional filename to include in the data URI.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public SendMessageRequestBuilder WithAttachmentFromBytes(byte[] bytes, string mimeType, string? filename = null)
    {
        return WithAttachment(filename != null
            ? Base64Attachment.FromDataUri(bytes, mimeType, filename)
            : Base64Attachment.FromDataUri(bytes, mimeType));
    }

    /// <summary>
    /// Adds a base64-encoded attachment to the message.
    /// </summary>
    /// <param name="attachment">The base64-encoded attachment string or data URI.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public SendMessageRequestBuilder WithAttachment(string attachment)
    {
        _request = _request with { Attachments = [.. _request.Attachments ?? [], attachment] };
        return this;
    }

    /// <summary>
    /// Sets the timestamp for editing an existing message.
    /// </summary>
    /// <param name="editTimestamp">The timestamp of the message to edit.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public SendMessageRequestBuilder WithEditTimestamp(DateTime editTimestamp)
    {
        _request = _request with { EditTimestamp = editTimestamp };
        return this;
    }

    /// <summary>
    /// Sets the link preview configuration for the message.
    /// </summary>
    /// <param name="linkPreview">The link preview configuration object.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public SendMessageRequestBuilder WithLinkPreview(LinkPreview linkPreview)
    {
        _request = _request with { LinkPreview = linkPreview };
        return this;
    }

    /// <summary>
    /// Creates and sets a link preview with the specified properties.
    /// </summary>
    /// <param name="url">The URL to preview.</param>
    /// <param name="title">Optional title for the link preview.</param>
    /// <param name="description">Optional description for the link preview.</param>
    /// <param name="thumbnail">Optional thumbnail image as a byte array.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public SendMessageRequestBuilder WithLinkPreview(string url,
        string? title = null,
        string? description = null,
        byte[]? thumbnail = null)
    {
        var linkPreview = new LinkPreview
        {
            Url = url,
            Title = title,
            Description = description,
            Thumbnail = thumbnail is not null ? Base64String.FromBytes(thumbnail) : string.Empty
        };
        return WithLinkPreview(linkPreview);
    }

    /// <summary>
    /// Sets the mentions in the message, either overwriting existing mentions or appending to them.
    /// </summary>
    /// <param name="mentions">The collection of mentions to add.</param>
    /// <param name="overwrite">If true, replaces existing mentions; if false, appends to them.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public SendMessageRequestBuilder WithMentions(IEnumerable<MessageMention> mentions, bool overwrite = true)
    {
        if (overwrite)
        {
            _request = _request with { Mentions = mentions.ToArray() };
        }
        else
        {
            _request = _request with { Mentions = [.. _request.Mentions ?? [], ..mentions] };
        }

        return this;
    }

    /// <summary>
    /// Adds a single mention to the message.
    /// </summary>
    /// <param name="mention">The mention to add.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public SendMessageRequestBuilder WithMention(MessageMention mention)
    {
        _request = _request with { Mentions = [.. _request.Mentions ?? [], mention] };
        return this;
    }

    /// <summary>
    /// Adds a mention with the specified author and position in the message text.
    /// </summary>
    /// <param name="author">The phone number or identifier of the user being mentioned.</param>
    /// <param name="start">The starting character position of the mention in the message text.</param>
    /// <param name="length">The length of the mention text in characters.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public SendMessageRequestBuilder WithMention(string author, int start, int length)
    {
        return WithMention(new MessageMention { Author = author, Start = start, Length = length });
    }

    /// <summary>
    /// Sets the message text content.
    /// </summary>
    /// <param name="message">The text content of the message.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public SendMessageRequestBuilder WithMessage(string message)
    {
        _request = _request with { Message = message };
        return this;
    }

    /// <summary>
    /// Sets whether the sender should receive a notification for their own message.
    /// </summary>
    /// <param name="notifySelf">If true, the sender receives a notification for this message.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public SendMessageRequestBuilder WithNotifySelf(bool notifySelf = true)
    {
        _request = _request with { NotifySelf = notifySelf };
        return this;
    }

    /// <summary>
    /// Sets the phone number of the sender.
    /// </summary>
    /// <param name="number">The sender's phone number.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public SendMessageRequestBuilder WithNumber(string number)
    {
        _request = _request with { Number = number };
        return this;
    }

    /// <summary>
    /// Sets the author of the quoted message when replying.
    /// </summary>
    /// <param name="quoteAuthor">The phone number or identifier of the quoted message's author.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public SendMessageRequestBuilder WithQuoteAuthor(string quoteAuthor)
    {
        _request = _request with { QuoteAuthor = quoteAuthor };
        return this;
    }

    /// <summary>
    /// Sets the mentions in the quoted message, either overwriting existing quote mentions or appending to them.
    /// </summary>
    /// <param name="quoteMentions">The collection of mentions in the quoted message.</param>
    /// <param name="overwrite">If true, replaces existing quote mentions; if false, appends to them.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public SendMessageRequestBuilder WithQuoteMentions(IEnumerable<MessageMention> quoteMentions, bool overwrite = true)
    {
        if (overwrite)
        {
            _request = _request with { QuoteMentions = quoteMentions.ToArray() };
        }
        else
        {
            _request = _request with { QuoteMentions = [.. _request.QuoteMentions ?? [], ..quoteMentions] };
        }

        return this;
    }

    /// <summary>
    /// Adds a single mention to the quoted message.
    /// </summary>
    /// <param name="mention">The mention in the quoted message.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public SendMessageRequestBuilder WithQuoteMention(MessageMention mention)
    {
        _request = _request with { QuoteMentions = [.. _request.QuoteMentions ?? [], mention] };
        return this;
    }

    /// <summary>
    /// Sets the text content of the message being quoted/replied to.
    /// </summary>
    /// <param name="quoteMessage">The text of the quoted message.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public SendMessageRequestBuilder WithQuoteMessage(string quoteMessage)
    {
        _request = _request with { QuoteMessage = quoteMessage };
        return this;
    }

    /// <summary>
    /// Sets the timestamp of the message being quoted/replied to.
    /// </summary>
    /// <param name="quoteTimestamp">The timestamp of the quoted message.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public SendMessageRequestBuilder WithQuoteTimestamp(DateTime quoteTimestamp)
    {
        _request = _request with { QuoteTimestamp = quoteTimestamp };
        return this;
    }

    /// <summary>
    /// Sets the recipients of the message, either overwriting existing recipients or appending to them.
    /// </summary>
    /// <param name="recipients">The collection of recipient phone numbers or group IDs.</param>
    /// <param name="overwrite">If true, replaces existing recipients; if false, appends to them.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public SendMessageRequestBuilder WithRecipients(IEnumerable<string> recipients, bool overwrite = true)
    {
        if (overwrite)
        {
            _request = _request with { Recipients = recipients.ToArray() };
        }
        else
        {
            _request = _request with { Recipients = [.. _request.Recipients ?? [], ..recipients] };
        }

        return this;
    }

    /// <summary>
    /// Adds a single recipient to the message.
    /// </summary>
    /// <param name="recipient">The phone number or group ID of the recipient.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public SendMessageRequestBuilder WithRecipient(string recipient)
    {
        _request = _request with { Recipients = [.. _request.Recipients ?? [], recipient] };
        return this;
    }

    /// <summary>
    /// Sets a sticker to be sent with the message.
    /// </summary>
    /// <param name="sticker">The sticker identifier or data.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public SendMessageRequestBuilder WithSticker(string sticker)
    {
        _request = _request with { Sticker = sticker };
        return this;
    }

    /// <summary>
    /// Sets the text formatting mode for the message.
    /// </summary>
    /// <param name="textMode">The text mode (e.g., styled, normal).</param>
    /// <returns>The builder instance for method chaining.</returns>
    public SendMessageRequestBuilder WithTextMode(TextMode textMode)
    {
        _request = _request with { TextMode = textMode };
        return this;
    }

    /// <summary>
    /// Sets whether the message should be viewable only once before disappearing.
    /// </summary>
    /// <param name="viewOnce">If true, the message can only be viewed once.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public SendMessageRequestBuilder WithViewOnce(bool viewOnce = true)
    {
        _request = _request with { ViewOnce = viewOnce };
        return this;
    }

    /// <summary>
    /// Builds and returns the configured SendMessageRequest.
    /// </summary>
    /// <returns>The configured SendMessageRequest instance.</returns>
    internal SendMessageRequest Build() => _request;
}