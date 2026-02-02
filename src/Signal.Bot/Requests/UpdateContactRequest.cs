namespace Signal.Bot.Requests;

public record UpdateContactRequest(string Number) : RequestBase($"v1/contacts/{Number}", HttpMethod.Put)
{
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("recipient")] public string? Recipient { get; set; }
    [JsonPropertyName("expiration_in_seconds")] public int? ExpirationTimeInSeconds { get; set; }
}