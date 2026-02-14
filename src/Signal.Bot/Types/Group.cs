using System.Text.Json.Serialization;

namespace Signal.Bot.Types;

public class Group
{
    [JsonPropertyName("name")] public string? Name { get; set; }

    [JsonPropertyName("admins")] public List<string>? Admins { get; set; }

    [JsonPropertyName("blocked")] public bool? Blocked { get; set; }

    [JsonPropertyName("description")] public string? Description { get; set; }

    [JsonPropertyName("id")] public string? Id { get; set; }

    [JsonPropertyName("internal_id")] public string? InternalId { get; set; }

    [JsonPropertyName("invite_link")] public string? InviteLink { get; set; }

    [JsonPropertyName("members")] public List<string>? Members { get; set; }

    [JsonPropertyName("pending_invites")] public List<string>? PendingInvites { get; set; }

    [JsonPropertyName("pending_requests")] public List<string>? PendingRequests { get; set; }
}