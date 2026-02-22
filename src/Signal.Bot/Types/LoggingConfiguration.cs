using System.Text.Json;
using System.Text.Json.Serialization;
using Signal.Bot.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents the logging configuration settings for the Signal Bot API.
/// </summary>
public record LoggingConfiguration
{
    /// <summary>
    /// Gets or sets the logging level (e.g., "debug", "info", "warn", "error").
    /// </summary>
    [JsonPropertyName("Level")]
    public string? Level { get; set; }
    
    /// <inheritdoc />
    public override string ToString() => JsonSerializer.Serialize(this, JsonBotAPI.Get(GetType()));
}