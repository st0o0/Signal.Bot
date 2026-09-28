using System.Text.Json;
using System.Text.Json.Serialization;
using Signal.Bot.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents an edit to a previously sent message.
/// </summary>
public record EditMessage
{
    /// <summary>
    /// Gets or sets the updated data message content.
    /// </summary>
    [JsonPropertyName("dataMessage")]
    public DataMessage? DataMessage { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of the original message being edited.
    /// </summary>
    [JsonPropertyName("targetSentTimestamp")]
    public DateTime TargetSent { get; set; }

    /// <inheritdoc />
    public override string ToString() => JsonSerializer.Serialize(this, JsonBotAPI.Get(GetType()));
}
