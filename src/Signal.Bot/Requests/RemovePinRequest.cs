namespace Signal.Bot.Requests;

public class RemovePinRequest(string number) : RequestBase($"v1/accounts/{number}/pin")
{
    [JsonIgnore] public string Number => number;
    public override HttpMethod HttpMethod => HttpMethod.Delete;
}