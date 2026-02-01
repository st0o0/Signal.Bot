using System;

namespace Signal.Bot.Types;

public class RemoteDelete
{
    [JsonPropertyName("timestamp")] public DateTime Timestamp { get; set; }
}