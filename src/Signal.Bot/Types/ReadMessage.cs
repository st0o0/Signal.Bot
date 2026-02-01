using System;

namespace Signal.Bot.Types;

public class ReadMessage
{
    [JsonPropertyName("sender")] public string? Sender { get; set; }

    [JsonPropertyName("timestamp")] public DateTime Timestamp { get; set; }
}