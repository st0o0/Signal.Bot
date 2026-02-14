using System.Text.Json.Serialization;

namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to update contact information including display name and disappearing message timer.
/// </summary>
/// <param name="Number">The phone number of the Signal account updating the contact.</param>
public record UpdateContactRequest(string Number) : RequestBase($"v1/contacts/{Number}", HttpMethod.Put)
{
    /// <summary>
    /// Gets or sets the display name to assign to this contact.
    /// </summary>
    [JsonPropertyName("name")] 
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the phone number of the contact being updated.
    /// </summary>
    [JsonPropertyName("recipient")] 
    public string? Recipient { get; set; }

    /// <summary>
    /// Gets or sets the disappearing message timer in seconds. Set to 0 to disable disappearing messages.
    /// </summary>
    [JsonPropertyName("expiration_in_seconds")] 
    public int? ExpirationTimeInSeconds { get; set; }
}