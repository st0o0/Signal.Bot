namespace Signal.Bot.Requests;

public class RegisterNumberRequest(string number) : RequestBase($"v1/register/{number}")
{
    [JsonIgnore] public string Number => number;
    public string? Captcha { get; set; }
    public bool? UseVoice { get; set; }
}