using System.Text.Json.Serialization;

namespace Signal.Bot.Types;

public class ReadMessage
{
    [JsonPropertyName("sender")] public string? Sender { get; set; }

    [JsonPropertyName("senderNumber")] public string? SenderNumber { get; set; }

    [JsonPropertyName("senderUuid")] public Guid SenderUuid { get; set; }

    [JsonPropertyName("timestamp")] public DateTime Timestamp { get; set; }
}