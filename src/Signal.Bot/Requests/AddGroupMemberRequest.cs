namespace Signal.Bot.Requests;

public record AddGroupMemberRequest(string Number, string GroupId) : RequestBase($"v1/groups/{Number}/{GroupId}/members")
{
    public ICollection<string>? Members { get; set; }
}