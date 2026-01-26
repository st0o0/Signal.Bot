namespace Signal.Bot.Types;

public class About
{
    [JsonPropertyName("build")] public int? Build { get; set; }

    [JsonPropertyName("capabilities")] public IDictionary<string, ICollection<string>>? Capabilities { get; set; }

    [JsonPropertyName("mode")] public string? Mode { get; set; }

    [JsonPropertyName("version")] public string? Version { get; set; }

    [JsonPropertyName("versions")] public ICollection<string>? Versions { get; set; }
}