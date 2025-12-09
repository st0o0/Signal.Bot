namespace Signal.Bot.Requests;

public class RemoveGroupAdminRequest(string number, string groupId)
    : RequestBase($"v1/groups/{number}/{groupId}/admins")
{
    public override HttpMethod HttpMethod => HttpMethod.Delete;
    public ICollection<string>? Admins { get; set; }
}