namespace Signal.Bot.Requests;

public class QuitGroupRequest(string number, string groupId) : RequestBase($"v1/groups/{number}/{groupId}/quit")
{
    [JsonIgnore] public string Number => number;
    [JsonIgnore] public string GroupId => groupId;
}