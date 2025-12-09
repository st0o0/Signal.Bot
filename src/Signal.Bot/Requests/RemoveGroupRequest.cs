namespace Signal.Bot.Requests;

public class RemoveGroupRequest(string number, string groupId) : RequestBase($"v1/groups/{number}/{groupId}")
{
    [JsonIgnore] public string Number => number;
    [JsonIgnore] public string GroupId => groupId;
    public override HttpMethod HttpMethod => HttpMethod.Delete;
}