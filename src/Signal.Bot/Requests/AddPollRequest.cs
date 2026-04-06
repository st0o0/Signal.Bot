using System.Text.Json.Serialization;

namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to create a new poll
/// </summary>
/// <param name="Number">The phone number of the Signal account from which the poll will be created.</param>
public record AddPollRequest(string Number) : RequestBase($"v1/polls/{Number}", HttpMethod.Post)
{
    /// <summary>
    /// Gets or sets the indicator if multiple selections are allowed
    /// </summary>
    [JsonPropertyName("allow_multiple_selections")]
    public bool? AllowMultipleSelections { get; set; }
    
    /// <summary>
    /// Gets or sets the answers for this poll request
    /// </summary>
    [JsonPropertyName("answers")]
    public string[]? Answers { get; set; }
    
    /// <summary>
    /// Gets or sets the question of this poll
    /// </summary>
    [JsonPropertyName("question")]
    public string? Question { get; set; }

    /// <summary>
    /// Gets or sets the recipient for this poll
    /// </summary>
    [JsonPropertyName("recipient")]
    public string? Recipient { get; set; }
}