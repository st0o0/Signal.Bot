using System;
using System.Runtime.Serialization;

namespace Signal.Bot.Requests;

public class SendMessageRequest : RequestBase<SendMessage>
{
    public SendMessageRequest() : base("v2/send")
    {
    }

    [JsonPropertyName("base64_attachments")] public ICollection<string>? Attachments { get; set; }

    [JsonPropertyName("edit_timestamp")] public int? EditTimestamp { get; set; }

    [JsonPropertyName("link_preview")] public LinkPreviewType? LinkPreview { get; set; }

    [JsonPropertyName("mentions")] public ICollection<MessageMention>? Mentions { get; set; }

    [JsonPropertyName("message")] public string? Message { get; set; }

    [JsonPropertyName("notify_self")] public bool? NotifySelf { get; set; }

    [JsonPropertyName("number")] public string? Number { get; set; }

    [JsonPropertyName("quote_author")] public string? QuoteAuthor { get; set; }

    [JsonPropertyName("quote_mentions")] public ICollection<MessageMention>? QuoteMentions { get; set; }

    [JsonPropertyName("quote_message")] public string? QuoteMessage { get; set; }

    [JsonPropertyName("quote_timestamp")] public int? QuoteTimestamp { get; set; }

    [JsonPropertyName("recipients")] public ICollection<string>? Recipients { get; set; }

    [JsonPropertyName("sticker")] public string? Sticker { get; set; }

    [JsonPropertyName("text_mode")] public TextMode? TextMode { get; set; }

    [JsonPropertyName("view_once")] public bool? ViewOnce { get; set; }
}

public class SendMessage
{
    [JsonPropertyName("timestamp")] public DateTime Timestamp { get; set; }
}

public class MessageMention
{
    [JsonPropertyName("author")] public string? Author { get; set; } = null;

    [JsonPropertyName("length")] public int? Length { get; set; } = null;

    [JsonPropertyName("start")] public int? Start { get; set; } = null;
}

public class LinkPreviewType
{
    [JsonPropertyName("base64_thumbnail")] public string? Thumbnail { get; set; } = null;

    [JsonPropertyName("description")] public string? Description { get; set; } = null;

    [JsonPropertyName("title")] public string? Title { get; set; } = null;

    [JsonPropertyName("url")] public string? Url { get; set; } = null;
}

public enum TextMode
{
    [EnumMember(Value = "normal")] Normal = 0,

    [EnumMember(Value = "styled")] Styled = 1,
}