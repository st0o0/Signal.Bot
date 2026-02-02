namespace Signal.Bot.Requests;

public record RemoveGroupMemberRequest(string Number, string GroupId)
    : RequestBase($"v1/groups/{Number}/{GroupId}/members", HttpMethod.Delete)
{
    [JsonPropertyName("members")] public string[]? Members { get; set; }
}