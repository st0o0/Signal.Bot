namespace Signal.Bot.Types;

/// <summary>
/// Reaction to a message
/// </summary>
public class ReactionMessage
{
    [JsonPropertyName("emoji")] public string? Emoji { get; set; }

    [JsonPropertyName("targetAuthor")] public string? TargetAuthor { get; set; }

    [JsonPropertyName("targetAuthorNumber")]
    public string? TargetAuthorNumber { get; set; }

    [JsonPropertyName("targetAuthorUuid")] public string? TargetAuthorUuid { get; set; }

    [JsonPropertyName("targetSentTimestamp")]
    public long? TargetSentTimestamp { get; set; }

    [JsonPropertyName("isRemove")] public bool? IsRemove { get; set; }
}