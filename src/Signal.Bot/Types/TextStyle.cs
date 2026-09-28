using System.Text.Json;
using System.Text.Json.Serialization;
using Signal.Bot.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents a text formatting style applied to a range of characters in a message.
/// </summary>
public record TextStyle
{
    /// <summary>
    /// Gets or sets the number of characters this style applies to.
    /// </summary>
    [JsonPropertyName("length")]
    public int Length { get; set; }

    /// <summary>
    /// Gets or sets the zero-based starting character position of the styled range.
    /// </summary>
    [JsonPropertyName("start")]
    public int Start { get; set; }

    /// <summary>
    /// Gets or sets the style type.
    /// </summary>
    [JsonPropertyName("style")]
    public TextStyleType? Style { get; set; }

    /// <inheritdoc />
    public override string ToString() => JsonSerializer.Serialize(this, JsonBotAPI.Get(GetType()));
}
