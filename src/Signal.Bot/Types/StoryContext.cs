using System.Text.Json;
using System.Text.Json.Serialization;
using Signal.Bot.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents the context of a story message being replied to.
/// </summary>
public record StoryContext
{
    /// <summary>
    /// Gets or sets the phone number of the story author.
    /// </summary>
    [JsonPropertyName("authorNumber")]
    public string? AuthorNumber { get; set; }

    /// <summary>
    /// Gets or sets the UUID of the story author.
    /// </summary>
    [JsonPropertyName("authorUuid")]
    public Guid AuthorId { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the story was sent.
    /// </summary>
    [JsonPropertyName("sentTimestamp")]
    public DateTime Sent { get; set; }

    /// <inheritdoc />
    public override string ToString() => JsonSerializer.Serialize(this, JsonBotAPI.Get(GetType()));
}
