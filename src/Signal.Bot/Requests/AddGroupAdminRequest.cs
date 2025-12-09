namespace Signal.Bot.Requests;

public class AddGroupAdminRequest(string number, string groupId) : RequestBase($"v1/groups/{number}/{groupId}/admins")
{
    [JsonIgnore] public string Number => number;
    [JsonIgnore] public string GroupId => groupId;
    public ICollection<string>? Admins { get; set; }
}