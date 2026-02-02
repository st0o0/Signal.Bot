namespace Signal.Bot.Requests;

public record RateLimitChallengeRequest(string Number) : RequestBase($"v1/accounts/{Number}/rate-limit-challenge")
{
    [JsonPropertyName("captcha")] public string? Captcha { get; set; }

    [JsonPropertyName("challenge_token")] public string? ChallengeToken { get; set; }
}