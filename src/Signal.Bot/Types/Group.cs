using System.Text.Json.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents a Signal group with its members, settings, and metadata.
/// </summary>
public class Group
{
    /// <summary>
    /// Gets or sets the name of the group.
    /// </summary>
    [JsonPropertyName("name")] 
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the list of phone numbers of group administrators.
    /// </summary>
    [JsonPropertyName("admins")] 
    public List<string>? Admins { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this group is blocked.
    /// </summary>
    [JsonPropertyName("blocked")] 
    public bool? Blocked { get; set; }

    /// <summary>
    /// Gets or sets the description text for the group.
    /// </summary>
    [JsonPropertyName("description")] 
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the group.
    /// </summary>
    [JsonPropertyName("id")] 
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the internal identifier used by Signal for this group.
    /// </summary>
    [JsonPropertyName("internal_id")] 
    public string? InternalId { get; set; }

    /// <summary>
    /// Gets or sets the invitation link URL for joining the group.
    /// </summary>
    [JsonPropertyName("invite_link")] 
    public string? InviteLink { get; set; }

    /// <summary>
    /// Gets or sets the list of phone numbers of current group members.
    /// </summary>
    [JsonPropertyName("members")] 
    public List<string>? Members { get; set; }

    /// <summary>
    /// Gets or sets the list of phone numbers of users with pending invitations to the group.
    /// </summary>
    [JsonPropertyName("pending_invites")] 
    public List<string>? PendingInvites { get; set; }

    /// <summary>
    /// Gets or sets the list of phone numbers of users who have requested to join the group (when approval is required).
    /// </summary>
    [JsonPropertyName("pending_requests")] 
    public List<string>? PendingRequests { get; set; }
}