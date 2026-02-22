using System.Text.Json;
using System.Text.Json.Serialization;
using Signal.Bot.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents a quoted (replied-to) message within a Signal message.
/// </summary>
public record QuoteData
{
    /// <summary>
    /// Gets or sets the unique identifier of the quoted message.
    /// </summary>
    [JsonPropertyName("id")] 
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the phone number or identifier of the author of the quoted message.
    /// </summary>
    [JsonPropertyName("author")] 
    public string? Author { get; set; }

    /// <summary>
    /// Gets or sets the text content of the quoted message.
    /// </summary>
    [JsonPropertyName("text")] 
    public string? Text { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the quoted message was sent.
    /// </summary>
    [JsonPropertyName("timestamp")] 
    public DateTime Timestamp { get; set; }
    
    /// <inheritdoc />
    public override string ToString() => JsonSerializer.Serialize(this, JsonBotAPI.Get(GetType()));
}