namespace Signal.Bot.Requests;

public class RateLimitChallengeRequest(string number) : RequestBase($"v1/accounts/{number}/rate-limit-challenge")
{
    [JsonIgnore] public string Number => number;
    public string? Captcha { get; set; }
    public string? ChallengeToken { get; set; }
}