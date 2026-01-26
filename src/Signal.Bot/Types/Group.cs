namespace Signal.Bot.Types;

public class Group
{
    [JsonPropertyName("name")] public string? Name { get; set; }

    [JsonPropertyName("admins")] public ICollection<string>? Admins { get; set; }

    [JsonPropertyName("blocked")] public bool? Blocked { get; set; }

    [JsonPropertyName("description")] public string? Description { get; set; }

    [JsonPropertyName("id")] public string? Id { get; set; }

    [JsonPropertyName("internal_id")] public string? InternalId { get; set; }

    [JsonPropertyName("invite_link")] public string? InviteLink { get; set; }

    [JsonPropertyName("members")] public ICollection<string>? Members { get; set; }

    [JsonPropertyName("pending_invites")] public ICollection<string>? PendingInvites { get; set; }

    [JsonPropertyName("pending_requests")] public ICollection<string>? PendingRequests { get; set; }
}