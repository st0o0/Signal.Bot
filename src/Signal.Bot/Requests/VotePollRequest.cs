using System.Text.Json.Serialization;

namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to vote on a poll
/// </summary>
/// <param name="Number">The phone number of the Signal account to vote in the poll.</param>
public record VotePollRequest(string Number) : RequestBase($"v1/polls/{Number}/vote", HttpMethod.Post)
{
    /// <summary>
    /// Gets or sets the uuid or phone number of the poll author
    /// </summary>
    [JsonPropertyName("poll_author")]
    public string? PollAuthor { get; set; }
    
    /// <summary>
    /// Gets or sets the timestamp of the poll to delete
    /// </summary>
    [JsonPropertyName("poll_timestamp")]
    public DateTime Timestamp { get; set; }
    
    /// <summary>
    /// Gets or sets the recipient for this poll
    /// </summary>
    [JsonPropertyName("recipient")]
    public string? Recipient { get; set; }
    
    /// <summary>
    /// Gets or sets an array of answers to vote for
    /// </summary>
    [JsonPropertyName("selected_answers")]
    public int[]? SelectedAnswers { get; set; }
}