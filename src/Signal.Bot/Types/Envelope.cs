using System.Text.Json;
using System.Text.Json.Serialization;
using Signal.Bot.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents the envelope container for a received Signal message, containing metadata and message content.
/// </summary>
public record Envelope
{
    /// <summary>
    /// Gets or sets the source identifier of the message sender.
    /// </summary>
    [JsonPropertyName("source")]
    public string? Source { get; set; }

    /// <summary>
    /// Gets or sets the phone number of the message sender.
    /// </summary>
    [JsonPropertyName("sourceNumber")]
    public string? SourceNumber { get; set; }

    /// <summary>
    /// Gets or sets the UUID of the message sender.
    /// </summary>
    [JsonPropertyName("sourceUuid")]
    public Guid SourceId { get; set; }

    /// <summary>
    /// TBD
    /// </summary>
    [JsonPropertyName("sourceName")]
    public string? SourceName { get; set; }

    /// <summary>
    /// Gets or sets the sourceDevice of the message sender.
    /// </summary>
    [JsonPropertyName("sourceDevice")]
    public int SourceDevice { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the message was sent.
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// TBD
    /// </summary>
    [JsonPropertyName("serverReceivedTimestamp")]
    public DateTime ServerReceived { get; set; }

    /// <summary>
    /// TBD
    /// </summary>
    [JsonPropertyName("serverDeliveredTimestamp")]
    public DateTime ServerDelivered { get; set; }

    /// <summary>
    /// Gets or sets the data message content if this envelope contains a regular message.
    /// </summary>
    [JsonPropertyName("dataMessage")]
    public DataMessage? DataMessage { get; set; }

    /// <summary>
    /// Gets or sets the sync message content if this envelope contains a synchronization message from linked devices.
    /// </summary>
    [JsonPropertyName("syncMessage")]
    public SyncMessage? SyncMessage { get; set; }

    /// <summary>
    /// Gets or sets the typing indicator message if this envelope contains typing status.
    /// </summary>
    [JsonPropertyName("typingMessage")]
    public TypingMessage? TypingMessage { get; set; }

    /// <summary>
    /// Gets or sets the receipt message if this envelope contains read or delivery receipts.
    /// </summary>
    [JsonPropertyName("receiptMessage")]
    public ReceiptMessage? ReceiptMessage { get; set; }

    /// <summary>
    /// TBD
    /// </summary>
    [JsonPropertyName("callMessage")]
    public CallMessage? CallMessage { get; set; }
    
    /// <inheritdoc />
    public override string ToString() => JsonSerializer.Serialize(this, JsonBotAPI.Get(GetType()));
}