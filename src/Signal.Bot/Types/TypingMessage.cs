using System.Text.Json;
using System.Text.Json.Serialization;
using Signal.Bot.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents a typing indicator message showing that a user is composing a message.
/// </summary>
public record TypingMessage
{
    /// <summary>
    /// Gets or sets the timestamp when the typing indicator was sent.
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the action type (e.g., "started" for typing started, "stopped" for typing stopped).
    /// </summary>
    [JsonPropertyName("action")]
    public TypingAction Action { get; set; }

    /// <inheritdoc />
    public override string ToString() => JsonSerializer.Serialize(this, JsonBotAPI.Get(GetType()));
}

/// <summary>
/// Defines the typing indicator actions.
/// </summary>
public enum TypingAction
{
    /// <summary>
    /// The user has stopped typing.
    /// </summary>
    [JsonStringEnumMemberName("STOPPED")] Stopped = 0,

    /// <summary>
    /// The user has started typing.
    /// </summary>
    [JsonStringEnumMemberName("STARTED")] Started = 1,
}