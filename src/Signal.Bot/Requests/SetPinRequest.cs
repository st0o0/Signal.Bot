namespace Signal.Bot.Requests;

public class SetPinRequest(string number) : RequestBase($"v1/accounts/{number}/pin")
{
    [JsonIgnore] public string Number => number;
    public string? Pin { get; set; }
}
