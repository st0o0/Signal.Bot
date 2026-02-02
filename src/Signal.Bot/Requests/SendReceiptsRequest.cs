using System.Runtime.Serialization;

namespace Signal.Bot.Requests;

public record SendReceiptsRequest(string Number) : RequestBase($"/v1/receipts/{Number}")
{
    [JsonPropertyName("receipt_type")] public ReceiptType ReceiptType { get; set; }

    [JsonPropertyName("recipient")] public string? Recipient { get; set; }

    [JsonPropertyName("timestamp")] public DateTime Timestamp { get; set; }
}

public enum ReceiptType
{
    [EnumMember(Value = "")] Undefined = 0,

    [EnumMember(Value = "read")] Read = 1,

    [EnumMember(Value = "viewed")] Viewed = 2,
}