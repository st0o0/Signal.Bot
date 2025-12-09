namespace Signal.Bot.Requests;

public class SetUsernameRequest(string number) : RequestBase<SetUsername>($"v1/accounts/{number}/username")
{
    [JsonIgnore] public string Number => number;
    public string? Username { get; set; }
}