using System.Text.Json.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// TBD
/// </summary>
public enum CallType
{
    /// <summary>
    /// TBD
    /// </summary>
    [JsonStringEnumMemberName("AUDIO_CALL")] AudioCall,

    /// <summary>
    /// TBD
    /// </summary>
    [JsonStringEnumMemberName("VIDEO_CALL")] VideoCall
}