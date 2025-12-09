namespace Signal.Bot.Types;

public class About
{
    [JsonPropertyName("build")] public int? Build { get; set; } = null;

    [JsonPropertyName("capabilities")]
    public IDictionary<string, ICollection<string>>? Capabilities { get; set; } = null;

    [JsonPropertyName("mode")] public string? Mode { get; set; } = null;

    [JsonPropertyName("version")] public string? Version { get; set; } = null;

    [JsonPropertyName("versions")] public ICollection<string>? Versions { get; set; } = null;
}