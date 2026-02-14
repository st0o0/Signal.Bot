using System.Text.Json.Serialization;

namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to update the settings of an existing Signal group.
/// </summary>
/// <param name="Number">The phone number of the Signal account updating the group.</param>
/// <param name="GroupId">The unique identifier of the group to update.</param>
public record UpdateGroupRequest(string Number, string GroupId)
    : RequestBase($"/v1/groups/{Number}/{GroupId}", HttpMethod.Put)
{
    /// <summary>
    /// Gets or sets the base64-encoded avatar image for the group. Set to <see langword="null"/> or empty to remove the avatar.
    /// </summary>
    [JsonPropertyName("base64_avatar")] 
    public string? Avatar { get; set; }

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
    /// Gets or sets the disappearing message timer in seconds. Set to 0 to disable disappearing messages.
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
}