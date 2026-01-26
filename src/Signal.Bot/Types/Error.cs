namespace Signal.Bot.Types;

public class Error
{
    [JsonPropertyName("error")] public string? Message { get; set; }
}