namespace Signal.Bot.Types;

public class Group
{
    [JsonPropertyName("name")] public string? Name { get; set; } = null;
    
    [JsonPropertyName("admins")] public ICollection<string>? Admins { get; set; } = null;

    [JsonPropertyName("blocked")] public bool? Blocked { get; set; } = null;

    [JsonPropertyName("description")] public string? Description { get; set; } = null;

    [JsonPropertyName("id")] public string? Id { get; set; } = null;

    [JsonPropertyName("internal_id")] public string? InternalId { get; set; } = null;

    [JsonPropertyName("invite_link")] public string? InviteLink { get; set; } = null;

    [JsonPropertyName("members")] public ICollection<string>? Members { get; set; } = null;

    [JsonPropertyName("pending_invites")] public ICollection<string>? PendingInvites { get; set; } = null;

    [JsonPropertyName("pending_requests")] public ICollection<string>? PendingRequests { get; set; } = null;
}