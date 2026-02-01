namespace Signal.Bot.Types;

public class SyncMessage
{
    [JsonPropertyName("readMessages")] public ICollection<ReadMessage>? ReadMessages { get; set; }
}