namespace Signal.Bot.Types;

public class ReceivedMessage
{
    [JsonPropertyName("envelope")] public Envelope? Envelope { get; set; }

    [JsonPropertyName("account")] public string? Account { get; set; }

    [JsonPropertyName("subscription")] public int? Subscription { get; set; }
}

public class Envelope
{
    [JsonPropertyName("source")] public string? Source { get; set; }

    [JsonPropertyName("sourceNumber")] public required string SourceNumber { get; set; }

    [JsonPropertyName("sourceUuid")] public required string SourceUuid { get; set; }

    [JsonPropertyName("timestamp")] public long Timestamp { get; set; }

    [JsonPropertyName("dataMessage")] public DataMessage? DataMessage { get; set; }

    [JsonPropertyName("syncMessage")] public SyncMessage? SyncMessage { get; set; }

    [JsonPropertyName("typingMessage")] public TypingMessage? TypingMessage { get; set; }

    [JsonPropertyName("receiptMessage")] public ReceiptMessage? ReceiptMessage { get; set; }
}

public class DataMessage
{
    [JsonPropertyName("timestamp")] public long? Timestamp { get; set; }

    [JsonPropertyName("body")] public string? Body { get; set; }

    [JsonPropertyName("attachments")] public List<Attachment>? Attachments { get; set; }

    [JsonPropertyName("groupV2")] public GroupV2Info? GroupV2 { get; set; }

    [JsonPropertyName("reaction")] public ReactionData? Reaction { get; set; }

    [JsonPropertyName("mentions")] public List<Mention>? Mentions { get; set; }

    [JsonPropertyName("quote")] public QuoteData? Quote { get; set; }

    [JsonPropertyName("readMessages")] public List<ReadMessage>? ReadMessages { get; set; }

    [JsonPropertyName("viewOnce")] public bool? ViewOnce { get; set; }

    [JsonPropertyName("previews")] public List<PreviewData>? Previews { get; set; }
}

public class SyncMessage
{
    [JsonPropertyName("sentMessage")] public DataMessage? SentMessage { get; set; }
}

public class TypingMessage
{
    [JsonPropertyName("timestamp")] public long? Timestamp { get; set; }

    [JsonPropertyName("action")] public string? Action { get; set; }
}

public class ReceiptMessage
{
    [JsonPropertyName("timestamps")] public List<long>? Timestamps { get; set; }

    [JsonPropertyName("type")] public string? Type { get; set; }
}

public class Attachment
{
    [JsonPropertyName("id")] public string? Id { get; set; }

    [JsonPropertyName("filename")] public string? Filename { get; set; }

    [JsonPropertyName("contentType")] public string? ContentType { get; set; }

    [JsonPropertyName("size")] public long? Size { get; set; }
}

public class GroupV2Info
{
    [JsonPropertyName("id")] public string? Id { get; set; }

    [JsonPropertyName("name")] public string? Name { get; set; }

    [JsonPropertyName("revision")] public int? Revision { get; set; }
}

public class ReactionData
{
    [JsonPropertyName("emoji")] public string? Emoji { get; set; }

    [JsonPropertyName("remove")] public bool? Remove { get; set; }

    [JsonPropertyName("targetAuthor")] public string? TargetAuthor { get; set; }

    [JsonPropertyName("targetSentTimestamp")]
    public long? TargetSentTimestamp { get; set; }
}

public class Mention
{
    [JsonPropertyName("start")] public int? Start { get; set; }

    [JsonPropertyName("length")] public int? Length { get; set; }

    [JsonPropertyName("uuid")] public string? Uuid { get; set; }
}

public class QuoteData
{
    [JsonPropertyName("id")] public long? Id { get; set; }

    [JsonPropertyName("author")] public string? Author { get; set; }

    [JsonPropertyName("text")] public string? Text { get; set; }

    [JsonPropertyName("timestamp")] public long? Timestamp { get; set; }
}

public class ReadMessage
{
    [JsonPropertyName("sender")] public string? Sender { get; set; }

    [JsonPropertyName("timestamp")] public long? Timestamp { get; set; }
}

public class PreviewData
{
    [JsonPropertyName("url")] public string? Url { get; set; }

    [JsonPropertyName("title")] public string? Title { get; set; }

    [JsonPropertyName("description")] public string? Description { get; set; }

    [JsonPropertyName("image")] public Attachment? Image { get; set; }
}