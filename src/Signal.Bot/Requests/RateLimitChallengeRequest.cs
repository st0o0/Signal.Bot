using System.Text.Json.Serialization;

namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to complete a rate limit challenge when Signal requires additional verification due to excessive API usage.
/// </summary>
/// <param name="Number">The phone number of the Signal account completing the rate limit challenge.</param>
public record RateLimitChallengeRequest(string Number) : RequestBase($"v1/accounts/{Number}/rate-limit-challenge")
{
    /// <summary>
    /// Gets or sets the solved CAPTCHA token obtained from the CAPTCHA provider.
    /// </summary>
    [JsonPropertyName("captcha")] 
    public string? Captcha { get; set; }

    /// <summary>
    /// Gets or sets the challenge token provided by Signal when the rate limit is triggered.
    /// </summary>
    [JsonPropertyName("challenge_token")] 
    public string? ChallengeToken { get; set; }
}