using Signal.Bot.Types;

namespace Signal.Bot.Requests;

public class GetContactsRequest(string number) : RequestBase<ICollection<Contact>>($"v1/contacts/{number}")
{
    [JsonIgnore] public string Number => number;
    public override HttpMethod HttpMethod => HttpMethod.Get;
}
