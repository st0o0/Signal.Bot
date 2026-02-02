namespace Signal.Bot.Requests;

public record SendReceiptsRequest(string Number) : RequestBase($"/v1/receipts/{Number}")
{
    [JsonPropertyName("receipt_type")] public ReceiptType ReceiptType { get; set; }

    [JsonPropertyName("recipient")] public string? Recipient { get; set; }

    [JsonPropertyName("timestamp")] public DateTime Timestamp { get; set; }
}

public enum ReceiptType
{
    [JsonStringEnumMemberName("UNDEFINED")] Undefined = 0,

    [JsonStringEnumMemberName("read")] Read = 1,

    [JsonStringEnumMemberName("viewed")] Viewed = 2,
}