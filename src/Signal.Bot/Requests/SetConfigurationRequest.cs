using System.Text.Json.Serialization;

namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to update the Signal Bot API configuration settings.
/// </summary>
public record SetConfigurationRequest() : RequestBase("v1/configuration")
{
    /// <summary>
    /// Gets or sets the logging configuration for the Signal Bot API.
    /// </summary>
    /// <seealso cref="Logging"/>
    [JsonPropertyName("logging")] 
    public Logging? Logging { get; set; }
}

/// <summary>
/// Defines logging configuration settings for the Signal Bot API.
/// </summary>
public class Logging
{
    /// <summary>
    /// Gets or sets the logging level (e.g., "debug", "info", "warn", "error").
    /// </summary>
    [JsonPropertyName("Level")] 
    public string? Level { get; set; }
}