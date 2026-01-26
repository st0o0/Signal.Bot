namespace Signal.Bot.Types;

public class ProfileCapabilities
{
    [JsonPropertyName("gv2")] public bool? Gv2 { get; set; }

    [JsonPropertyName("storage")] public bool? Storage { get; set; }

    [JsonPropertyName("gv1-migration")] public bool? Gv1Migration { get; set; }

    [JsonPropertyName("senderKey")] public bool? SenderKey { get; set; }

    [JsonPropertyName("announcementGroup")] public bool? AnnouncementGroup { get; set; }

    [JsonPropertyName("changeNumber")] public bool? ChangeNumber { get; set; }

    [JsonPropertyName("stories")] public bool? Stories { get; set; }

    [JsonPropertyName("giftBadges")] public bool? GiftBadges { get; set; }
}