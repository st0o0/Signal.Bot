namespace Signal.Bot.Requests;

public record RateLimitChallengeRequest(string Number) : RequestBase($"v1/accounts/{Number}/rate-limit-challenge")
{
    public string? Captcha { get; set; }
    public string? ChallengeToken { get; set; }
}