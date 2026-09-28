using System.Text.Json;
using System.Text.Json.Serialization;
using Signal.Bot.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents a text attachment used in story messages.
/// </summary>
public record TextAttachment
{
    /// <summary>
    /// Gets or sets the background color.
    /// </summary>
    [JsonPropertyName("backgroundColor")]
    public string? BackgroundColor { get; set; }

    /// <summary>
    /// Gets or sets the background gradient configuration.
    /// </summary>
    [JsonPropertyName("backgroundGradient")]
    public BackgroundGradient? BackgroundGradient { get; set; }

    /// <summary>
    /// Gets or sets the link preview for this text attachment.
    /// </summary>
    [JsonPropertyName("preview")]
    public Preview? Preview { get; set; }

    /// <summary>
    /// Gets or sets the text style.
    /// </summary>
    [JsonPropertyName("style")]
    public TextStyleType? Style { get; set; }

    /// <summary>
    /// Gets or sets the text content.
    /// </summary>
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    /// <summary>
    /// Gets or sets the text background color.
    /// </summary>
    [JsonPropertyName("textBackgroundColor")]
    public string? TextBackgroundColor { get; set; }

    /// <summary>
    /// Gets or sets the text foreground color.
    /// </summary>
    [JsonPropertyName("textForegroundColor")]
    public string? TextForegroundColor { get; set; }

    /// <inheritdoc />
    public override string ToString() => JsonSerializer.Serialize(this, JsonBotAPI.Get(GetType()));
}
