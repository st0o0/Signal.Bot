using System.Text.Json;
using System.Text.Json.Serialization;
using Signal.Bot.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents a background gradient used in story text attachments.
/// </summary>
public record BackgroundGradient
{
    /// <summary>
    /// Gets or sets the gradient angle in degrees.
    /// </summary>
    [JsonPropertyName("angle")]
    public int? Angle { get; set; }

    /// <summary>
    /// Gets or sets the list of gradient colors.
    /// </summary>
    [JsonPropertyName("colors")]
    public List<string>? Colors { get; set; }

    /// <summary>
    /// Gets or sets the end color of the gradient.
    /// </summary>
    [JsonPropertyName("endColor")]
    public string? EndColor { get; set; }

    /// <summary>
    /// Gets or sets the position stops for each gradient color.
    /// </summary>
    [JsonPropertyName("positions")]
    public List<double>? Positions { get; set; }

    /// <summary>
    /// Gets or sets the start color of the gradient.
    /// </summary>
    [JsonPropertyName("startColor")]
    public string? StartColor { get; set; }

    /// <inheritdoc />
    public override string ToString() => JsonSerializer.Serialize(this, JsonBotAPI.Get(GetType()));
}
