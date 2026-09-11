using System.Text.Json.Serialization;

namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to pin a message in a Signal group.
/// </summary>
/// <param name="Number">The phone number of the Signal account.</param>
/// <param name="GroupId">The unique identifier of the group.</param>
public record PinMessageRequest(string Number, string GroupId)
    : RequestBase($"v1/groups/{Number}/{GroupId}/pin-message")
{
    /// <summary>
    /// Gets or sets the phone number of the message author.
    /// </summary>
    [JsonPropertyName("target_author")]
    public string? TargetAuthor { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of the message to pin.
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the optional duration for the pin.
    /// </summary>
    [JsonPropertyName("duration")]
    public int? Duration { get; set; }
}
