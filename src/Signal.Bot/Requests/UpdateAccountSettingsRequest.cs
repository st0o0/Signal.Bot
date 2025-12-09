namespace Signal.Bot.Requests;

public class UpdateAccountSettingsRequest(string number) : RequestBase($"v1/accounts/{number}/settings")
{
    [JsonIgnore] public string Number => number;
    public override HttpMethod HttpMethod => HttpMethod.Put;

    public bool DiscoverableByNumber { get; set; }
    public bool ShareNumberWithContacts { get; set; }
}
