using System.Text.Json.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// 
/// </summary>
public record CallMessage
{
    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("hangupMessage")]
    public HangupMessage? HangupMessage { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("offerMessage")]
    public OfferMessage? OfferMessage { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("iceUpdateMessages")]
    public IceUpdateMessage[]? IceUpdateMessages { get; set; }
}