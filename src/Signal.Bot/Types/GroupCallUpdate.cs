using System.Text.Json;
using System.Text.Json.Serialization;
using Signal.Bot.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents a group call update event.
/// </summary>
public record GroupCallUpdate
{
    /// <summary>
    /// Gets or sets the era ID of the group call.
    /// </summary>
    [JsonPropertyName("eraId")]
    public string? EraId { get; set; }

    /// <inheritdoc />
    public override string ToString() => JsonSerializer.Serialize(this, JsonBotAPI.Get(GetType()));
}
