using System.Text.Json.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents the permission settings for a Signal group.
/// </summary>
public record GroupPermissions
{
    /// <summary>
    /// Gets or sets the permission level for adding members.
    /// </summary>
    [JsonPropertyName("add_members")]
    public string? AddMembers { get; set; }

    /// <summary>
    /// Gets or sets the permission level for editing group settings.
    /// </summary>
    [JsonPropertyName("edit_group")]
    public string? EditGroup { get; set; }

    /// <summary>
    /// Gets or sets the permission level for sending messages.
    /// </summary>
    [JsonPropertyName("send_messages")]
    public string? SendMessages { get; set; }
}
