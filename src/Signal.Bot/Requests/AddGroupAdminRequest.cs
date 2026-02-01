namespace Signal.Bot.Requests;

public record AddGroupAdminRequest(string Number, string GroupId) : RequestBase($"v1/groups/{Number}/{GroupId}/admins")
{
    public ICollection<string>? Admins { get; set; }
}