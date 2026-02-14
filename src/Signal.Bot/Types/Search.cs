using System.Text.Json.Serialization;

namespace Signal.Bot.Types;

public class Search
{
    [JsonPropertyName("number")] public string? Number { get; set; }

    [JsonPropertyName("registered")] public bool? Registered { get; set; }
}