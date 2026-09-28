using System.Text.Json;
using System.Text.Json.Serialization;
using Signal.Bot.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents a poll creation event in a received message.
/// </summary>
public record PollCreate
{
    /// <summary>
    /// Gets or sets a value indicating whether multiple option selections are allowed.
    /// </summary>
    [JsonPropertyName("allowMultiple")]
    public bool? AllowMultiple { get; set; }

    /// <summary>
    /// Gets or sets the list of poll options.
    /// </summary>
    [JsonPropertyName("options")]
    public List<string>? Options { get; set; }

    /// <summary>
    /// Gets or sets the poll question text.
    /// </summary>
    [JsonPropertyName("question")]
    public string? Question { get; set; }

    /// <inheritdoc />
    public override string ToString() => JsonSerializer.Serialize(this, JsonBotAPI.Get(GetType()));
}
