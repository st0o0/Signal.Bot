using Signal.Bot.Types;

namespace Signal.Bot.Requests;

public class GetGroupRequest(string number, string groupId)
    : RequestBase<Group>($"v1/groups/{number}/{groupId}")
{
    [JsonIgnore] public string Number => number;
    [JsonIgnore] public string GroupId => groupId;
    public override HttpMethod HttpMethod => HttpMethod.Get;
}