using System.Text.Json.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents the logging configuration settings for the Signal Bot API.
/// </summary>
public class LoggingConfiguration
{
    /// <summary>
    /// Gets or sets the logging level (e.g., "debug", "info", "warn", "error").
    /// </summary>
    [JsonPropertyName("Level")]
    public string? Level { get; set; }
}