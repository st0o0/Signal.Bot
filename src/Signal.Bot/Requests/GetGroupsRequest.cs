using Signal.Bot.Types;

namespace Signal.Bot.Requests;

public class GetGroupsRequest(string number) : RequestBase<ICollection<Group>>($"v1/groups/{number}")
{
    public GetGroupsRequest() : this(string.Empty) { }

    [JsonIgnore] public string Number => number;

    public override HttpMethod HttpMethod => HttpMethod.Get;
}
