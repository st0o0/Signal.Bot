using System.Text.Json;
using System.Text.Json.Serialization;
using Signal.Bot.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents a message containing read or delivery receipts for one or more messages.
/// </summary>
public record ReceiptMessage
{
    /// <summary>
    /// Gets or sets the list of timestamps of messages for which receipts are being sent.
    /// </summary>
    [JsonPropertyName("timestamps")] 
    public List<DateTime>? Timestamps { get; set; }

    /// <summary>
    /// Gets or sets the type of receipt (e.g., "read", "delivery", "viewed").
    /// </summary>
    [JsonPropertyName("type")] 
    public string? Type { get; set; }
    
    /// <inheritdoc />
    public override string ToString() => JsonSerializer.Serialize(this, JsonBotAPI.Get(GetType()));
}