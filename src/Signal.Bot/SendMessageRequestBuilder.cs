using System;
using System.Linq;
using Signal.Bot.Requests;

namespace Signal.Bot;

public class SendMessageRequestBuilder
{
    private SendMessageRequest _request = new();

    public static SendMessageRequestBuilder Create() => new();

    public SendMessageRequestBuilder WithAttachmentFromFile(string filePath, bool includeFilename = false)
    {
        return WithAttachment(Base64Attachment.FromFile(filePath, includeFilename: includeFilename));
    }

    public SendMessageRequestBuilder WithAttachmentFromBytes(byte[] bytes, string mimeType, string? filename = null)
    {
        if (filename != null)
        {
            return WithAttachment(Base64Attachment.FromDataUri(bytes, mimeType, filename));
        }

        return WithAttachment(Base64Attachment.FromDataUri(bytes, mimeType));
    }

    public SendMessageRequestBuilder WithAttachment(string attachment)
    {
        _request = _request with { Attachments = [.. _request.Attachments ?? [], attachment] };
        return this;
    }

    public SendMessageRequestBuilder WithEditTimestamp(DateTime editTimestamp)
    {
        _request = _request with { EditTimestamp = editTimestamp };
        return this;
    }

    public SendMessageRequestBuilder WithLinkPreview(LinkPreviewType linkPreview)
    {
        _request = _request with { LinkPreview = linkPreview };
        return this;
    }

    public SendMessageRequestBuilder WithLinkPreview(string url,
        string? title = null,
        string? description = null,
        byte[]? thumbnail = null)
    {
        var linkPreview = new LinkPreviewType
        {
            Url = url,
            Title = title,
            Description = description,
            Thumbnail = thumbnail is not null ? Base64String.FromBytes(thumbnail) : string.Empty
        };
        return WithLinkPreview(linkPreview);
    }

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

    public SendMessageRequestBuilder WithMention(MessageMention mention)
    {
        _request = _request with { Mentions = [.. _request.Mentions ?? [], mention] };
        return this;
    }

    public SendMessageRequestBuilder WithMention(string author, int start, int length)
    {
        return WithMention(new MessageMention { Author = author, Start = start, Length = length });
    }

    public SendMessageRequestBuilder WithMessage(string message)
    {
        _request = _request with { Message = message };
        return this;
    }

    public SendMessageRequestBuilder WithNotifySelf(bool notifySelf = true)
    {
        _request = _request with { NotifySelf = notifySelf };
        return this;
    }

    public SendMessageRequestBuilder WithNumber(string number)
    {
        _request = _request with { Number = number };
        return this;
    }

    public SendMessageRequestBuilder WithQuoteAuthor(string quoteAuthor)
    {
        _request = _request with { QuoteAuthor = quoteAuthor };
        return this;
    }

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

    public SendMessageRequestBuilder WithQuoteMention(MessageMention mention)
    {
        _request = _request with { QuoteMentions = [.. _request.QuoteMentions ?? [], mention] };
        return this;
    }

    public SendMessageRequestBuilder WithQuoteMessage(string quoteMessage)
    {
        _request = _request with { QuoteMessage = quoteMessage };
        return this;
    }

    public SendMessageRequestBuilder WithQuoteTimestamp(DateTime quoteTimestamp)
    {
        _request = _request with { QuoteTimestamp = quoteTimestamp };
        return this;
    }

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

    public SendMessageRequestBuilder WithRecipient(string recipient)
    {
        _request = _request with { Recipients = [.. _request.Recipients ?? [], recipient] };
        return this;
    }

    public SendMessageRequestBuilder WithSticker(string sticker)
    {
        _request = _request with { Sticker = sticker };
        return this;
    }

    public SendMessageRequestBuilder WithTextMode(TextMode textMode)
    {
        _request = _request with { TextMode = textMode };
        return this;
    }

    public SendMessageRequestBuilder WithViewOnce(bool viewOnce = true)
    {
        _request = _request with { ViewOnce = viewOnce };
        return this;
    }

    public SendMessageRequest Build() => _request;
}