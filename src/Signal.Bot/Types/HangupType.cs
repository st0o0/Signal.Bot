using System.Text.Json.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// TBD
/// </summary>
public enum HangupType
{
    /// <summary>
    /// TBD
    /// </summary>
    [JsonStringEnumMemberName("NORMAL")] Normal,

    /// <summary>
    /// TBD
    /// </summary>
    [JsonStringEnumMemberName("ACCEPTED")] Accepted,

    /// <summary>
    /// TBD
    /// </summary>
    [JsonStringEnumMemberName("DECLINED")] Declined,

    /// <summary>
    /// TBD
    /// </summary>
    [JsonStringEnumMemberName("BUSY")] Busy,

    /// <summary>
    /// TBD
    /// </summary>
    [JsonStringEnumMemberName("NEED_PERMISSION")] NeedPermission
}