namespace Signal.Bot.Requests;

public class TrustIdentityRequest(string number, string verifiedNumber)
    : RequestBase($"v1/identities/{number}/trust/{verifiedNumber}")
{
    [JsonIgnore] public string Number => number;
    [JsonIgnore] public string VerifiedNumber => verifiedNumber;
    public override HttpMethod HttpMethod => HttpMethod.Put;
    public bool? TrustAllKnownKeys { get; set; }
    public string? VerifiedSafetyNumber { get; set; }
}