using System.Text.Json;
using System.Text.Json.Serialization;
using Signal.Bot.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents an error response from the Signal Bot API.
/// </summary>
public record ErrorResponse
{
    /// <summary>
    /// Gets or sets the error message describing what went wrong.
    /// </summary>
    [JsonPropertyName("error")]
    public string? Message { get; set; }

    /// <inheritdoc />
    public override string ToString() => JsonSerializer.Serialize(this, JsonBotAPI.Get(GetType()));
}