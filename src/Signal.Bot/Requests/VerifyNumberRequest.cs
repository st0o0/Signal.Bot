namespace Signal.Bot.Requests;

public class VerifyNumberRequest(string number, string token) : RequestBase<string>($"v1/register/{number}/verify/{token}")
{
    [JsonIgnore] public string Number => number;
    [JsonIgnore] public string Token => token;
    public string? Pin { get; set; }
}