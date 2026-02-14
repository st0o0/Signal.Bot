using System.Text.Json.Serialization;

namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to set or update the registration lock PIN for the Signal account.
/// </summary>
/// <param name="Number">The phone number of the Signal account for which the PIN is being set.</param>
public record SetPinRequest(string Number) : RequestBase($"v1/accounts/{Number}/pin")
{
    /// <summary>
    /// Gets or sets the PIN to set for registration lock (typically 4-8 digits).
    /// </summary>
    [JsonPropertyName("pin")] 
    public string? Pin { get; set; }
}