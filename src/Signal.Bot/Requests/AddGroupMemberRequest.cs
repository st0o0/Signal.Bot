namespace Signal.Bot.Requests;

public class AddGroupMemberRequest(string number, string groupId) : RequestBase($"v1/groups/{number}/{groupId}/members")
{
    [JsonIgnore] public string Number => number;
    [JsonIgnore] public string GroupId => groupId;
    public ICollection<string>? Members { get; set; }
}
