using System.Text.Json.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// 
/// </summary>
public record OfferMessage
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

    /// <summary>
    /// TBD
    /// </summary>
    [JsonPropertyName("type")]
    public CallType Type { get; set; }
}