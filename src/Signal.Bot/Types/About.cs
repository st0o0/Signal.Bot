using System.Text.Json.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents information about the Signal Bot API server including version, build, and capabilities.
/// </summary>
public class About
{
    /// <summary>
    /// Gets or sets the build number of the Signal Bot API server.
    /// </summary>
    [JsonPropertyName("build")]
    public int? Build { get; set; }

    /// <summary>
    /// Gets or sets the capabilities supported by the Signal Bot API server, organized by category.
    /// Each category maps to a collection of supported capability names.
    /// </summary>
    [JsonPropertyName("capabilities")] public Dictionary<string, List<string>>? Capabilities { get; set; }

    /// <summary>
    /// Gets or sets the operational mode of the Signal Bot API server (e.g., "normal", "native", "json-rpc").
    /// </summary>
    [JsonPropertyName("mode")]
    public string? Mode { get; set; }

    /// <summary>
    /// Gets or sets the current version string of the Signal Bot API server.
    /// </summary>
    [JsonPropertyName("version")]
    public string? Version { get; set; }

    /// <summary>
    /// Gets or sets the collection of API versions supported by the Signal Bot API server.
    /// </summary>
    [JsonPropertyName("versions")] public List<string>? Versions { get; set; }
}