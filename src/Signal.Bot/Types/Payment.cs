using System.Text.Json;
using System.Text.Json.Serialization;
using Signal.Bot.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents a payment message.
/// </summary>
public record Payment
{
    /// <summary>
    /// Gets or sets the optional note attached to the payment.
    /// </summary>
    [JsonPropertyName("note")]
    public string? Note { get; set; }

    /// <summary>
    /// Gets or sets the payment receipt data.
    /// </summary>
    [JsonPropertyName("receipt")]
    public string? Receipt { get; set; }

    /// <inheritdoc />
    public override string ToString() => JsonSerializer.Serialize(this, JsonBotAPI.Get(GetType()));
}
