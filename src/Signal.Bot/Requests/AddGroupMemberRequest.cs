namespace Signal.Bot.Requests;

public record AddGroupMemberRequest(string Number, string GroupId)
    : RequestBase($"v1/groups/{Number}/{GroupId}/members")
{
    [JsonPropertyName("members")] public string[]? Members { get; set; }
}