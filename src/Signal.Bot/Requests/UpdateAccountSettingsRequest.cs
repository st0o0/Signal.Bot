using System.Text.Json.Serialization;

namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to update the account privacy and discoverability settings.
/// </summary>
/// <param name="Number">The phone number of the Signal account whose settings are being updated.</param>
public record UpdateAccountSettingsRequest(string Number)
    : RequestBase($"v1/accounts/{Number}/settings", HttpMethod.Put)
{
    /// <summary>
    /// Gets or sets whether the account can be discovered by others using the phone number.
    /// </summary>
    [JsonPropertyName("discoverable_by_number")] 
    public bool DiscoverableByNumber { get; set; }

    /// <summary>
    /// Gets or sets whether to share the phone number with contacts.
    /// </summary>
    [JsonPropertyName("share_number")] 
    public bool ShareNumber { get; set; }
}