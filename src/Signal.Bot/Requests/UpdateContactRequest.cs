namespace Signal.Bot.Requests;

public class UpdateContactRequest(string number) : RequestBase($"v1/contacts/{number}")
{
    [JsonIgnore] public string Number => number;
    public override HttpMethod HttpMethod => HttpMethod.Put;

    public string? Name { get; set; }
    public string? Recipient { get; set; }
    public int? ExpirationTime { get; set; }
}