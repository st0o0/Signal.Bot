namespace Signal.Bot.Requests;

public record UpdateGroupRequest(string Number, string GroupId)
    : RequestBase($"/v1/groups/{Number}/{GroupId}", HttpMethod.Put)
{
    [JsonPropertyName("base64_avatar")] public string? Avatar { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("description")] public string? Description { get; set; }

    [JsonPropertyName("expiration_time")] public int? ExpirationTime { get; set; }

    [JsonPropertyName("group_link")] public GroupLink GroupLink { get; set; }

    [JsonPropertyName("permissions")] public Permissions Permissions { get; set; }
}