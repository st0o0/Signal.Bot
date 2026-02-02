using System.Runtime.Serialization;

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
    [EnumMember(Value = "")] Undefined = 0,

    [EnumMember(Value = "UNTRUSTED")] Untrusted = 1,

    [EnumMember(Value = "TRUSTED_UNVERIFIED")] TrustedUnverified = 2,

    [EnumMember(Value = "TRUSTED_VERIFIED")] TrustedVerified = 3
}