namespace Signal.Bot.Requests;

public record RemoveReactionRequest(string Number) : RequestBase<string>($"v1/reactions/{Number}", HttpMethod.Delete)
{
    [JsonPropertyName("reaction")] public string? Reaction { get; set; }

    [JsonPropertyName("recipient")] public string? Recipient { get; set; }

    [JsonPropertyName("target_author")] public string? TargetAuthor { get; set; }

    [JsonPropertyName("timestamp")] public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}