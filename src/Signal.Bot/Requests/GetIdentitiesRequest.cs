using Signal.Bot.Types;

namespace Signal.Bot.Requests;

public class GetIdentitiesRequest(string number) : RequestBase<ICollection<Identity>>($"v1/identities/{number}")
{
    [JsonIgnore] public string Number => number;
    public override HttpMethod HttpMethod => HttpMethod.Get;
}