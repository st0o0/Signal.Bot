using System.Text.Json.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents a Signal identity with cryptographic fingerprint and trust status information for end-to-end encryption verification.
/// </summary>
public class Identity
{
    /// <summary>
    /// Gets or sets the date and time when this identity was first added or discovered.
    /// </summary>
    [JsonPropertyName("added")] public DateTime Added { get; set; }

    /// <summary>
    /// Gets or sets the cryptographic fingerprint of the identity key.
    /// </summary>
    [JsonPropertyName("fingerprint")]
    public string? Fingerprint { get; set; }

    /// <summary>
    /// Gets or sets the phone number associated with this identity.
    /// </summary>
    [JsonPropertyName("number")]
    public string? Number { get; set; }

    /// <summary>
    /// Gets or sets the safety number used for manual identity verification between users.
    /// Safety numbers can be compared in person or through a trusted channel to verify identity.
    /// </summary>
    [JsonPropertyName("safety_number")]
    public string? SafetyNumber { get; set; }

    /// <summary>
    /// Gets or sets the trust status of this identity.
    /// See <see cref="IdentityStatus"/> for possible values.
    /// </summary>
    [JsonPropertyName("status")]
    public IdentityStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier (UUID) for this identity.
    /// </summary>
    [JsonPropertyName("uuid")]
    public Guid Id { get; set; }
}

/// <summary>
/// Defines the trust status of a Signal identity (safety number) for end-to-end encryption verification.
/// Values: Undefined (unknown status), Untrusted (not verified), TrustedUnverified (trusted but not manually verified), TrustedVerified (manually verified through safety number comparison).
/// </summary>
public enum IdentityStatus
{
    /// <summary>
    /// Undefined or unknown identity status.
    /// </summary>
    [JsonStringEnumMemberName("UNDEFINED")] Undefined = 0,

    /// <summary>
    /// The identity is not trusted and has not been verified.
    /// </summary>
    [JsonStringEnumMemberName("UNTRUSTED")] Untrusted = 1,

    /// <summary>
    /// The identity is trusted but has not been manually verified through safety number comparison.
    /// </summary>
    [JsonStringEnumMemberName("TRUSTED_UNVERIFIED")] TrustedUnverified = 2,

    /// <summary>
    /// The identity is trusted and has been manually verified by comparing safety numbers.
    /// </summary>
    [JsonStringEnumMemberName("TRUSTED_VERIFIED")] TrustedVerified = 3
}