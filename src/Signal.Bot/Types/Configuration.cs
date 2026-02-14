using System.Text.Json.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents the configuration settings for the Signal Bot API.
/// </summary>
public class Configuration
{
    /// <summary>
    /// Gets or sets the logging configuration for the API.
    /// </summary>
    [JsonPropertyName("logging")] 
    public LoggingConfiguration? Logging { get; set; } = null;
}