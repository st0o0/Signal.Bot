using System.Text.Json.Serialization;
using Signal.Bot.Requests;

namespace Signal.Bot.Types;

/// <summary>
/// Represents a response to a <see cref="AddPollRequest"/>
/// </summary>
public record PollResponse()
{
    /// <summary>
    /// Gets or sets the timestamp
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }
}