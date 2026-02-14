using System.Text.Json.Serialization;

namespace Signal.Bot.Types;

public class DataMessage
{
    [JsonPropertyName("timestamp")] public DateTime Timestamp { get; set; }

    [JsonPropertyName("message")] public string? Message { get; set; }

    [JsonPropertyName("attachments")] public List<Attachment>? Attachments { get; set; }

    [JsonPropertyName("groupV2")] public GroupV2Info? GroupV2 { get; set; }

    [JsonPropertyName("reaction")] public ReactionData? Reaction { get; set; }

    [JsonPropertyName("mentions")] public List<Mention>? Mentions { get; set; }

    [JsonPropertyName("quote")] public QuoteData? Quote { get; set; }

    [JsonPropertyName("readMessages")] public List<ReadMessage>? ReadMessages { get; set; }

    [JsonPropertyName("viewOnce")] public bool? ViewOnce { get; set; }

    [JsonPropertyName("expiresInSeconds")] public int? ExpiresInSeconds { get; set; }

    [JsonPropertyName("isExpirationUpdate")] public bool? IsExpirationUpdate { get; set; }

    [JsonPropertyName("previews")] public List<PreviewData>? Previews { get; set; }
}