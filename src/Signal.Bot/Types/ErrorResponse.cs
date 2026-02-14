using System.Text.Json.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents an error response from the Signal Bot API.
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Gets or sets the error message describing what went wrong.
    /// </summary>
    [JsonPropertyName("error")] 
    public string? Message { get; set; }
}