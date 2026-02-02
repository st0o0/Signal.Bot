namespace Signal.Bot.Requests;

public record AddReactionRequest(string Number) : RequestBase($"v1/reactions/{Number}")
{
    [JsonPropertyName("reaction")] public string? Reaction { get; set; }

    [JsonPropertyName("recipient")] public string? Recipient { get; set; }

    [JsonPropertyName("target_author")] public string? TargetAuthor { get; set; }

    [JsonPropertyName("timestamp")] public DateTime Timestamp { get; set; }
}