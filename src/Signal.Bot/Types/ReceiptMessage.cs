using System;

namespace Signal.Bot.Types;

public class ReceiptMessage
{
    [JsonPropertyName("timestamps")] public List<DateTime>? Timestamps { get; set; }

    [JsonPropertyName("type")] public string? Type { get; set; }
}