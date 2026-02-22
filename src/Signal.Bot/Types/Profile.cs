using System.Text.Json;
using System.Text.Json.Serialization;
using Signal.Bot.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents a Signal user's profile information including name, about text, and avatar status.
/// </summary>
public record Profile
{
    /// <summary>
    /// Gets or sets the given name (first name) of the user.
    /// </summary>
    [JsonPropertyName("given_name")] 
    public string? GivenName { get; set; }

    /// <summary>
    /// Gets or sets the last name (family name) of the user.
    /// </summary>
    [JsonPropertyName("lastname")] 
    public string? Lastname { get; set; }

    /// <summary>
    /// Gets or sets the about/status text displayed on the user's profile.
    /// </summary>
    [JsonPropertyName("about")] 
    public string? About { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user has set a profile avatar image.
    /// </summary>
    [JsonPropertyName("has_avatar")] 
    public bool? HasAvatar { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the profile was last updated.
    /// </summary>
    [JsonPropertyName("last_updated_timestamp")] 
    public DateTime LastUpdated { get; set; }
    
    /// <inheritdoc />
    public override string ToString() => JsonSerializer.Serialize(this, JsonBotAPI.Get(GetType()));
}