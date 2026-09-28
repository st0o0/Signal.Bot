using System.Text.Json.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Defines the types of group update events in Signal.
/// </summary>
public enum GroupInfoType
{
    /// <summary>
    /// A message was delivered to the group.
    /// </summary>
    [JsonStringEnumMemberName("DELIVER")] Deliver,

    /// <summary>
    /// The group metadata was updated (name, avatar, members, etc.).
    /// </summary>
    [JsonStringEnumMemberName("UPDATE")] Update,

    /// <summary>
    /// A member left the group.
    /// </summary>
    [JsonStringEnumMemberName("QUIT")] Quit
}
