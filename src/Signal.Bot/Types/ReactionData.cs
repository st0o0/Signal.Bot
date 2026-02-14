using System.Text.Json.Serialization;

namespace Signal.Bot.Types;

public class ReactionData
{
    [JsonPropertyName("emoji")] public string? Emoji { get; set; }

    [JsonPropertyName("remove")] public bool? Remove { get; set; }

    [JsonPropertyName("targetAuthor")] public string? TargetAuthor { get; set; }

    [JsonPropertyName("targetSentTimestamp")] public DateTime TargetSent { get; set; }
}