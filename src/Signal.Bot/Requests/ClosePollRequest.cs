using System.Text.Json.Serialization;

namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to close a poll
/// </summary>
/// <param name="Number">The phone number of the Signal account which will close the poll.</param>
public record ClosePollRequest(string Number) : RequestBase($"v1/polls/{Number}", HttpMethod.Delete)
{
    /// <summary>
    /// Gets or sets the timestamp of the poll to close
    /// </summary>
    [JsonPropertyName("poll_timestamp")]
    public DateTime Timestamp { get; set; }
    
    /// <summary>
    /// Gets or sets the recipient for this poll
    /// </summary>
    [JsonPropertyName("recipient")]
    public string? Recipient { get; set; }
}