namespace Signal.Bot.Types;

public class Profile
{
    [JsonPropertyName("given_name")] public string? GivenName { get; set; }

    [JsonPropertyName("lastname")] public string? Lastname { get; set; }
    
    [JsonPropertyName("about")] public string? About { get; set; }
    
    [JsonPropertyName("has_avatar")] public bool? HasAvatar { get; set; }

    [JsonPropertyName("last_updated_timestamp")]
    public long? LastUpdatedTimestamp { get; set; }
}