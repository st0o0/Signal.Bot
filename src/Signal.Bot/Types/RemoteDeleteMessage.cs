namespace Signal.Bot.Types;

/// <summary>
/// Remote delete message
/// </summary>
public class RemoteDeleteMessage
{
    [JsonPropertyName("targetSentTimestamp")]
    public long TargetSentTimestamp { get; set; }
}