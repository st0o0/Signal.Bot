using System.Text.Json.Serialization;
using Signal.Bot.Types;

namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to create a new Signal group with specified settings and initial members.
/// </summary>
/// <param name="Number">The phone number of the Signal account creating the group.</param>
public record CreateGroupRequest(string Number) : RequestBase<Group>($"v1/groups/{Number}")
{
    /// <summary>
    /// Gets or sets the name of the group.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the description text for the group.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the disappearing message timer in seconds. Set to 0 to disable.
    /// </summary>
    [JsonPropertyName("expiration_time")]
    public int? ExpirationTime { get; set; }

    /// <summary>
    /// Gets or sets the group link access level, controlling how users can join via link.
    /// </summary>
    /// <seealso cref="GroupLink"/>
    [JsonPropertyName("group_link")]
    public GroupLink GroupLink { get; set; }

    /// <summary>
    /// Gets or sets the permission settings for group actions.
    /// </summary>
    /// <seealso cref="Permissions"/>
    [JsonPropertyName("permissions")]
    public Permissions? Permissions { get; set; }

    /// <summary>
    /// Gets or sets the array of phone numbers of initial group members.
    /// </summary>
    [JsonPropertyName("members")]
    public string[]? Members { get; set; }
}

/// <summary>
/// Defines permission settings for various group actions, controlling which members can perform specific operations.
/// </summary>
public class Permissions
{
    /// <summary>
    /// Gets or sets who can add new members to the group.
    /// </summary>
    [JsonPropertyName("add_members")]
    public GroupPermission AddMembers { get; set; }

    /// <summary>
    /// Gets or sets who can edit group information (name, description, avatar).
    /// </summary>
    [JsonPropertyName("edit_group")]
    public GroupPermission EditGroup { get; set; }

    /// <summary>
    /// Gets or sets who can send messages in the group.
    /// </summary>
    [JsonPropertyName("send_messages")]
    public GroupPermission SendMessages { get; set; }
}

/// <summary>
/// Defines the permission levels for group actions.
/// </summary>
public enum GroupPermission
{
    /// <summary>
    /// Only group administrators can perform the action.
    /// </summary>
    [JsonStringEnumMemberName("only-admins")] OnlyAdmins = 1,

    /// <summary>
    /// Every group member can perform the action.
    /// </summary>
    [JsonStringEnumMemberName("every-member")] EveryMember = 2,
}

/// <summary>
/// Defines the access levels for group invitation links.
/// </summary>
public enum GroupLink
{
    /// <summary>
    /// Group link is disabled; users cannot join via link.
    /// </summary>
    [JsonStringEnumMemberName("disabled")] Disabled = 1,

    /// <summary>
    /// Group link is enabled; anyone with the link can join immediately.
    /// </summary>
    [JsonStringEnumMemberName("enabled")] Enabled = 2,

    /// <summary>
    /// Group link is enabled but requires administrator approval before joining.
    /// </summary>
    [JsonStringEnumMemberName("enabled-with-approval")] EnabledWithApproval = 3,
}