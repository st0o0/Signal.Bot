using System.Text.Json.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Defines the text formatting styles that can be applied to message text.
/// </summary>
public enum TextStyleType
{
    /// <summary>
    /// Bold text formatting.
    /// </summary>
    [JsonStringEnumMemberName("bold")] Bold,

    /// <summary>
    /// Italic text formatting.
    /// </summary>
    [JsonStringEnumMemberName("italic")] Italic,

    /// <summary>
    /// Strikethrough text formatting.
    /// </summary>
    [JsonStringEnumMemberName("strikethrough")] Strikethrough,

    /// <summary>
    /// Monospace (code) text formatting.
    /// </summary>
    [JsonStringEnumMemberName("monospace")] Monospace,

    /// <summary>
    /// Spoiler text that is hidden until tapped.
    /// </summary>
    [JsonStringEnumMemberName("spoiler")] Spoiler
}
