using System.Text.Json.Serialization;

namespace Signal.Bot.Types;

public class Device
{
    [JsonPropertyName("name")] public string? Name { get; set; }

    [JsonPropertyName("creation_timestamp")] public DateTime Created { get; set; }

    [JsonPropertyName("last_seen_timestamp")] public DateTime LastSeen { get; set; }
}