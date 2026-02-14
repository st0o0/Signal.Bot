using System.Text.Json.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents the feature capabilities supported by a Signal user's client.
/// </summary>
public class ProfileCapabilities
{
    /// <summary>
    /// Gets or sets a value indicating whether the user supports Signal Groups v2.
    /// </summary>
    [JsonPropertyName("gv2")] 
    public bool? Gv2 { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user supports Signal's cloud storage feature.
    /// </summary>
    [JsonPropertyName("storage")] 
    public bool? Storage { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user supports migration from Groups v1 to v2.
    /// </summary>
    [JsonPropertyName("gv1-migration")] 
    public bool? Gv1Migration { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user supports sender key distribution for efficient group messaging.
    /// </summary>
    [JsonPropertyName("senderKey")] 
    public bool? SenderKey { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user supports announcement-only groups.
    /// </summary>
    [JsonPropertyName("announcementGroup")] 
    public bool? AnnouncementGroup { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user supports changing their phone number while keeping their account.
    /// </summary>
    [JsonPropertyName("changeNumber")] 
    public bool? ChangeNumber { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user supports Signal Stories feature.
    /// </summary>
    [JsonPropertyName("stories")] 
    public bool? Stories { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user supports sending and receiving gift badges.
    /// </summary>
    [JsonPropertyName("giftBadges")] 
    public bool? GiftBadges { get; set; }
}