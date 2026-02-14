using System.Text.Json.Serialization;

namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to send a read receipt, delivery receipt, or viewed receipt for a message.
/// </summary>
/// <param name="Number">The phone number of the Signal account sending the receipt.</param>
public record SendReceiptsRequest(string Number) : RequestBase($"/v1/receipts/{Number}")
{
    /// <summary>
    /// Gets or sets the type of receipt to send (read, viewed, etc.).
    /// </summary>
    /// <seealso cref="ReceiptType"/>
    [JsonPropertyName("receipt_type")] 
    public ReceiptType ReceiptType { get; set; }

    /// <summary>
    /// Gets or sets the phone number of the message sender to whom the receipt is being sent.
    /// </summary>
    [JsonPropertyName("recipient")] 
    public string? Recipient { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of the message for which the receipt is being sent.
    /// </summary>
    [JsonPropertyName("timestamp")] 
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// Defines the types of message receipts that can be sent in Signal.
/// </summary>
public enum ReceiptType
{
    /// <summary>
    /// Read receipt indicating the message has been read by the recipient.
    /// </summary>
    [JsonStringEnumMemberName("read")] 
    Read = 1,

    /// <summary>
    /// Viewed receipt indicating the message content (typically media) has been viewed.
    /// </summary>
    [JsonStringEnumMemberName("viewed")] 
    Viewed = 2,
}