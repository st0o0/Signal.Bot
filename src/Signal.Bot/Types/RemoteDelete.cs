namespace Signal.Bot.Types;

public class RemoteDelete
{
    [JsonPropertyName("timestamp")] public long? Timestamp { get; set; }
}