namespace Signal.Bot.Types;

public class Identity
{
    [JsonPropertyName("uuid")] public Guid Id { get; set; }

    [JsonPropertyName("added")] public DateTime Added { get; set; }

    [JsonPropertyName("fingerprint")] public string? Fingerprint { get; set; }

    [JsonPropertyName("number")] public string? Number { get; set; }

    [JsonPropertyName("safety_number")] public string? SafetyNumber { get; set; }

    [JsonPropertyName("status")] public IdentityStatus Status { get; set; }
}

public enum IdentityStatus
{
    [JsonStringEnumMemberName("UNDEFINED")] Undefined = 0,

    [JsonStringEnumMemberName("UNTRUSTED")] Untrusted = 1,

    [JsonStringEnumMemberName("TRUSTED_UNVERIFIED")] TrustedUnverified = 2,

    [JsonStringEnumMemberName("TRUSTED_VERIFIED")] TrustedVerified = 3
}