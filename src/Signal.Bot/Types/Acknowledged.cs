namespace Signal.Bot.Types;

public class Acknowledged
{
    [JsonPropertyName("timestamp")] public DateTime Timestamp { get; set; }
}