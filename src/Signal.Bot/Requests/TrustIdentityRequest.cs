namespace Signal.Bot.Requests;

public record TrustIdentityRequest(string Number, string VerifiedNumber)
    : RequestBase($"v1/identities/{Number}/trust/{VerifiedNumber}", HttpMethod.Put)
{
    public bool? TrustAllKnownKeys { get; set; }
    public string? VerifiedSafetyNumber { get; set; }
}