namespace Signal.Bot.Types;

public class SyncMessage
{
    [JsonPropertyName("sentMessage")] public DataMessage? SentMessage { get; set; }
}