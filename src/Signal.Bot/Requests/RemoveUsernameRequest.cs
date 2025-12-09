namespace Signal.Bot.Requests;

public class RemoveUsernameRequest(string number) : RequestBase<object>($"v1/accounts/{number}/username")
{
    [JsonIgnore] public string Number => number;
    public override HttpMethod HttpMethod => HttpMethod.Delete;
}
