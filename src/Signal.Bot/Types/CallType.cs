using System.Text.Json.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Defines the types of calls that can be made in Signal.
/// </summary>
public enum CallType
{
    /// <summary>
    /// An audio-only voice call.
    /// </summary>
    [JsonStringEnumMemberName("AUDIO_CALL")] AudioCall,

    /// <summary>
    /// A video call.
    /// </summary>
    [JsonStringEnumMemberName("VIDEO_CALL")] VideoCall
}
