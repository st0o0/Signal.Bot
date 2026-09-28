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
    /// Gets or sets the display name of the message sender.
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
    /// Gets or sets the timestamp when the server received the message.
    /// </summary>
    [JsonPropertyName("serverReceivedTimestamp")]
    public DateTime ServerReceived { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the server delivered the message.
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
    /// Gets or sets the call message if this envelope contains a voice or video call event.
    /// </summary>
    [JsonPropertyName("callMessage")]
    public CallMessage? CallMessage { get; set; }

    /// <summary>
    /// Gets or sets the edit message if this envelope contains an edit to a previously sent message.
    /// </summary>
    [JsonPropertyName("editMessage")]
    public EditMessage? EditMessage { get; set; }

    /// <summary>
    /// Gets or sets the story message if this envelope contains a story.
    /// </summary>
    [JsonPropertyName("storyMessage")]
    public StoryMessage? StoryMessage { get; set; }

    /// <inheritdoc />
    public override string ToString() => JsonSerializer.Serialize(this, JsonBotAPI.Get(GetType()));
}