using System.Text.Json;
using System.Text.Json.Serialization;
using Signal.Bot.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents a sticker in a received message.
/// </summary>
public record Sticker
{
    /// <summary>
    /// Gets or sets the sticker pack identifier.
    /// </summary>
    [JsonPropertyName("packId")]
    public string? PackId { get; set; }

    /// <summary>
    /// Gets or sets the sticker identifier within the pack.
    /// </summary>
    [JsonPropertyName("stickerId")]
    public int? StickerId { get; set; }

    /// <inheritdoc />
    public override string ToString() => JsonSerializer.Serialize(this, JsonBotAPI.Get(GetType()));
}
