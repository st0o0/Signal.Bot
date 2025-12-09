namespace Signal.Bot.Types;

/// <summary>
/// Generic error response
/// </summary>
public class Error
{
    [JsonPropertyName("error")] public string? Message { get; set; }
}