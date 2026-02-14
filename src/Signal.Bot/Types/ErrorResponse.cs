using System.Text.Json.Serialization;

namespace Signal.Bot.Types;

public class ErrorResponse
{
    [JsonPropertyName("error")] public string? Message { get; set; }
}