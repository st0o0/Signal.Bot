using System.Text.Json;
using System.Text.Json.Serialization;
using Signal.Bot.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents the result of setting a username, including the username and its shareable link.
/// </summary>
public record SetUsername
{
    /// <summary>
    /// Gets or sets the username that was set.
    /// </summary>
    [JsonPropertyName("username")]
    public string? Username { get; set; }

    /// <summary>
    /// Gets or sets the shareable link for the username (e.g., signal.me/username).
    /// </summary>
    [JsonPropertyName("username_link")]
    public string? UsernameLink { get; set; }

    /// <inheritdoc />
    public override string ToString() => JsonSerializer.Serialize(this, JsonBotAPI.Get(GetType()));
}