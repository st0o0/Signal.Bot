namespace Signal.Bot.Types;

public class About
{
    [JsonPropertyName("build")] public int? Build { get; set; }

    [JsonPropertyName("capabilities")] public Dictionary<string, List<string>>? Capabilities { get; set; }

    [JsonPropertyName("mode")] public string? Mode { get; set; }

    [JsonPropertyName("version")] public string? Version { get; set; }

    [JsonPropertyName("versions")] public List<string>? Versions { get; set; }
}