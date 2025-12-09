namespace Signal.Bot.Types;

public class Device
{
    [JsonPropertyName("name")] public string? Name { get; set; }

    [JsonPropertyName("creation_timestamp")] public long? Created { get; set; }

    [JsonPropertyName("last_seen_timestamp")] public long? LastSeen { get; set; }
}