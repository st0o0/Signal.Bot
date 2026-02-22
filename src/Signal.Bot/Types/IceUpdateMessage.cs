using System.Text.Json.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// TBD
/// </summary>
public record IceUpdateMessage
{
    /// <summary>
    /// TBD
    /// </summary>
    [JsonPropertyName("id")]
    public long? Id { get; set; }

    /// <summary>
    /// TBD
    /// </summary>
    [JsonPropertyName("opaque")]
    public string? Opaque { get; set; }
}