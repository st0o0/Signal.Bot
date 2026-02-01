namespace Signal.Bot.Requests;

public record RegisterNumberRequest(string Number) : RequestBase($"v1/register/{Number}")
{
    public string? Captcha { get; set; }
    public bool? UseVoice { get; set; }
}