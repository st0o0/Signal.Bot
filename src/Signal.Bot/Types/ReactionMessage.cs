using System;

namespace Signal.Bot.Types;

public class ReactionMessage
{
    [JsonPropertyName("emoji")] public string? Emoji { get; set; }

    [JsonPropertyName("targetAuthor")] public string? TargetAuthor { get; set; }

    [JsonPropertyName("targetAuthorNumber")] public string? TargetAuthorNumber { get; set; }

    [JsonPropertyName("targetAuthorUuid")] public Guid TargetAuthorId { get; set; }

    [JsonPropertyName("targetSentTimestamp")] public DateTime TargetSent { get; set; }

    [JsonPropertyName("isRemove")] public bool? IsRemove { get; set; }
}