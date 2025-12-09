namespace Signal.Bot.Types;

/// <summary>
/// Profile information (from profile retrieval)
/// </summary>
public class ProfileResponse
{
    [JsonPropertyName("name")] public string? Name { get; set; }

    [JsonPropertyName("about")] public string? About { get; set; }

    [JsonPropertyName("aboutEmoji")] public string? AboutEmoji { get; set; }

    [JsonPropertyName("avatarUrlPath")] public string? AvatarUrlPath { get; set; }

    [JsonPropertyName("mobileCoinAddress")]
    public string? MobileCoinAddress { get; set; }

    [JsonPropertyName("unidentifiedAccess")]
    public string? UnidentifiedAccess { get; set; }

    [JsonPropertyName("unrestricted")] public bool? Unrestricted { get; set; }

    [JsonPropertyName("capabilities")] public ProfileCapabilities? Capabilities { get; set; }

    [JsonPropertyName("uuid")] public string? Uuid { get; set; }
}