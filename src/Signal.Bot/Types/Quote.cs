using System.Text.Json;
using System.Text.Json.Serialization;
using Signal.Bot.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents a quoted (replied-to) message within a Signal message.
/// </summary>
public record Quote
{
    /// <summary>
    /// Gets or sets the unique identifier of the quoted message.
    /// </summary>
    [JsonPropertyName("id")]
    public DateTime? Id { get; set; }

    /// <summary>
    /// Gets or sets the phone number or identifier of the author of the quoted message.
    /// </summary>
    [JsonPropertyName("author")]
    public string? Author { get; set; }

    /// <summary>
    /// TBD
    /// </summary>
    [JsonPropertyName("authorNumber")]
    public string? AuthorNumber { get; set; }

    /// <summary>
    /// TBD
    /// </summary>
    [JsonPropertyName("authorUuid")]
    public Guid? AuthorId { get; set; }

    /// <summary>
    /// Gets or sets the text content of the quoted message.
    /// </summary>
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    /// <summary>
    /// TBD
    /// </summary>
    [JsonPropertyName("mentions")]
    public List<Mention>? Mentions { get; set; }

    /// <summary>
    /// TBD
    /// </summary>
    [JsonPropertyName("attachments")]
    public List<Attachment>? Attachments { get; set; }

    /// <inheritdoc />
    public override string ToString() => JsonSerializer.Serialize(this, JsonBotAPI.Get(GetType()));
}