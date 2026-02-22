using System.Text.Json;
using System.Text.Json.Serialization;
using Signal.Bot.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents the configuration settings for the Signal Bot API.
/// </summary>
public record Configuration
{
    /// <summary>
    /// Gets or sets the logging configuration for the API.
    /// </summary>
    [JsonPropertyName("logging")]
    public LoggingConfiguration? Logging { get; set; }

    /// <inheritdoc />
    public override string ToString() => JsonSerializer.Serialize(this, JsonBotAPI.Get(GetType()));
}