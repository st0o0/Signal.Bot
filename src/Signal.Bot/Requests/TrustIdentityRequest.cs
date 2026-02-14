using System.Text.Json.Serialization;

namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to mark a contact's identity key as trusted after verifying their safety number.
/// </summary>
/// <param name="Number">The phone number of the Signal account trusting the identity.</param>
/// <param name="VerifiedNumber">The phone number of the contact whose identity is being verified and trusted.</param>
public record TrustIdentityRequest(string Number, string VerifiedNumber)
    : RequestBase($"v1/identities/{Number}/trust/{VerifiedNumber}", HttpMethod.Put)
{
    /// <summary>
    /// Gets or sets whether to trust all known keys for this contact. If <see langword="true"/>, trusts all existing keys without individual verification.
    /// </summary>
    [JsonPropertyName("trust_all_known_keys")] 
    public bool? TrustAllKnownKeys { get; set; }

    /// <summary>
    /// Gets or sets the safety number to verify, obtained through QR code scanning or manual comparison with the contact.
    /// </summary>
    [JsonPropertyName("verified_safety_number")] 
    public string? VerifiedSafetyNumber { get; set; }
}