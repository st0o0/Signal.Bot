namespace Signal.Bot.Requests;

public record RemoveGroupAdminRequest(string Number, string GroupId)
    : RequestBase($"v1/groups/{Number}/{GroupId}/admins", HttpMethod.Delete)
{
    [JsonPropertyName("admins")] public string[]? Admins { get; set; }
}