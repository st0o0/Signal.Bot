using System.Text.Json.Serialization;

namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to update the Signal profile information including name, about text, and avatar image.
/// </summary>
/// <param name="Number">The phone number of the Signal account whose profile is being updated.</param>
public record UpdateProfileRequest(string Number) : RequestBase($"v1/profiles/{Number}")
{
    /// <summary>
    /// Gets or sets the about/status text to display on the profile.
    /// </summary>
    [JsonPropertyName("about")] 
    public string? About { get; set; }

    /// <summary>
    /// Gets or sets the base64-encoded avatar image. Recommended formats are JPEG or PNG.
    /// </summary>
    [JsonPropertyName("base64_avatar")] 
    public string? Avatar { get; set; }

    /// <summary>
    /// Gets or sets the display name for the profile.
    /// </summary>
    [JsonPropertyName("name")] 
    public string? Name { get; set; }
}