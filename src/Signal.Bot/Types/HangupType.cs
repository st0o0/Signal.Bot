using System.Text.Json.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Defines the types of call hangup events in Signal.
/// </summary>
public enum HangupType
{
    /// <summary>
    /// The call ended normally.
    /// </summary>
    [JsonStringEnumMemberName("NORMAL")] Normal,

    /// <summary>
    /// The call was accepted.
    /// </summary>
    [JsonStringEnumMemberName("ACCEPTED")] Accepted,

    /// <summary>
    /// The call was declined by the recipient.
    /// </summary>
    [JsonStringEnumMemberName("DECLINED")] Declined,

    /// <summary>
    /// The recipient is busy on another call.
    /// </summary>
    [JsonStringEnumMemberName("BUSY")] Busy,

    /// <summary>
    /// The call requires permission to proceed.
    /// </summary>
    [JsonStringEnumMemberName("NEED_PERMISSION")] NeedPermission
}
