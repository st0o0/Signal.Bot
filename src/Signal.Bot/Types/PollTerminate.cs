using System.Text.Json;
using System.Text.Json.Serialization;
using Signal.Bot.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents a poll close/terminate event.
/// </summary>
public record PollTerminate
{
    /// <summary>
    /// Gets or sets the timestamp of the original poll message being closed.
    /// </summary>
    [JsonPropertyName("targetSentTimestamp")]
    public DateTime TargetSent { get; set; }

    /// <inheritdoc />
    public override string ToString() => JsonSerializer.Serialize(this, JsonBotAPI.Get(GetType()));
}
