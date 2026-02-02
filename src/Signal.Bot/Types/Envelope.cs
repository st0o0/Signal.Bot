namespace Signal.Bot.Types;

public class Envelope
{
    [JsonPropertyName("source")] public string? Source { get; set; }

    [JsonPropertyName("sourceNumber")] public string? SourceNumber { get; set; }

    [JsonPropertyName("sourceUuid")] public Guid SourceId { get; set; }

    [JsonPropertyName("timestamp")] public DateTime Timestamp { get; set; }

    [JsonPropertyName("dataMessage")] public DataMessage? DataMessage { get; set; }

    [JsonPropertyName("syncMessage")] public SyncMessage? SyncMessage { get; set; }

    [JsonPropertyName("typingMessage")] public TypingMessage? TypingMessage { get; set; }

    [JsonPropertyName("receiptMessage")] public ReceiptMessage? ReceiptMessage { get; set; }
}