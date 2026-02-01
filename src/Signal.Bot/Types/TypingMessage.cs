using System;

namespace Signal.Bot.Types;

public class TypingMessage
{
    [JsonPropertyName("timestamp")] public DateTime Timestamp { get; set; }

    [JsonPropertyName("action")] public string? Action { get; set; }
}