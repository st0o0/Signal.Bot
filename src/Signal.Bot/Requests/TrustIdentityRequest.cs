namespace Signal.Bot.Requests;

public record TrustIdentityRequest(string Number, string VerifiedNumber)
    : RequestBase($"v1/identities/{Number}/trust/{VerifiedNumber}", HttpMethod.Put)
{
    [JsonPropertyName("trust_all_known_keys")] public bool? TrustAllKnownKeys { get; set; }

    [JsonPropertyName("verified_safety_number")] public string? VerifiedSafetyNumber { get; set; }
}