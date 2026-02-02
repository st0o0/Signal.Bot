namespace Signal.Bot.Types;

public class QuoteData
{
    [JsonPropertyName("id")] public Guid Id { get; set; }

    [JsonPropertyName("author")] public string? Author { get; set; }

    [JsonPropertyName("text")] public string? Text { get; set; }

    [JsonPropertyName("timestamp")] public DateTime Timestamp { get; set; }
}