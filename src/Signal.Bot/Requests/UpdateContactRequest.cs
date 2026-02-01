namespace Signal.Bot.Requests;

public record UpdateContactRequest(string Number) : RequestBase($"v1/contacts/{Number}", HttpMethod.Put)
{
    public string? Name { get; set; }
    public string? Recipient { get; set; }
    [JsonPropertyName("expiration_in_seconds")] public int? ExpirationTimeInSeconds { get; set; }
}