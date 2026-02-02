using System.Runtime.Serialization;
using Signal.Bot.Types;

namespace Signal.Bot.Requests;

public record SendMessageRequest() : RequestBase<Acknowledged>("v2/send")
{
    [JsonPropertyName("base64_attachments")] public string[]? Attachments { get; set; }

    [JsonPropertyName("edit_timestamp")] public DateTime EditTimestamp { get; set; }

    [JsonPropertyName("link_preview")] public LinkPreview? LinkPreview { get; set; }

    [JsonPropertyName("mentions")] public MessageMention[]? Mentions { get; set; }

    [JsonPropertyName("message")] public string? Message { get; set; }

    [JsonPropertyName("notify_self")] public bool? NotifySelf { get; set; }

    [JsonPropertyName("number")] public string? Number { get; set; }

    [JsonPropertyName("quote_author")] public string? QuoteAuthor { get; set; }

    [JsonPropertyName("quote_mentions")] public MessageMention[]? QuoteMentions { get; set; }

    [JsonPropertyName("quote_message")] public string? QuoteMessage { get; set; }

    [JsonPropertyName("quote_timestamp")] public DateTime QuoteTimestamp { get; set; }

    [JsonPropertyName("recipients")] public string[]? Recipients { get; set; }

    [JsonPropertyName("sticker")] public string? Sticker { get; set; }

    [JsonPropertyName("text_mode")] public TextMode? TextMode { get; set; }

    [JsonPropertyName("view_once")] public bool? ViewOnce { get; set; }
}

public record MessageMention
{
    [JsonPropertyName("author")] public string? Author { get; set; }

    [JsonPropertyName("length")] public int? Length { get; set; }

    [JsonPropertyName("start")] public int? Start { get; set; }
}

public record LinkPreview
{
    [JsonPropertyName("base64_thumbnail")] public string? Thumbnail { get; set; }

    [JsonPropertyName("description")] public string? Description { get; set; }

    [JsonPropertyName("title")] public string? Title { get; set; }

    [JsonPropertyName("url")] public string? Url { get; set; }
}

public enum TextMode
{
    [EnumMember(Value = "normal")] Normal = 0,

    [EnumMember(Value = "styled")] Styled = 1,
}