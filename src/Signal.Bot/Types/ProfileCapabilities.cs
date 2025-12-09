namespace Signal.Bot.Types;

/// <summary>
/// Profile capabilities (features supported by the contact)
/// </summary>
public class ProfileCapabilities
{
    [JsonPropertyName("gv2")] public bool? Gv2 { get; set; } = null;

    [JsonPropertyName("storage")] public bool? Storage { get; set; } = null;

    [JsonPropertyName("gv1-migration")] public bool? Gv1Migration { get; set; } = null;

    [JsonPropertyName("senderKey")] public bool? SenderKey { get; set; } = null;

    [JsonPropertyName("announcementGroup")]
    public bool? AnnouncementGroup { get; set; } = null;

    [JsonPropertyName("changeNumber")] public bool? ChangeNumber { get; set; } = null;

    [JsonPropertyName("stories")] public bool? Stories { get; set; } = null;

    [JsonPropertyName("giftBadges")] public bool? GiftBadges { get; set; } = null;
}