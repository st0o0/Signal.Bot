namespace Signal.Bot.Requests;

public record UpdateAccountSettingsRequest(string Number)
    : RequestBase($"v1/accounts/{Number}/settings", HttpMethod.Put)
{
    [JsonPropertyName("discoverable_by_number")] public bool DiscoverableByNumber { get; set; }

    [JsonPropertyName("share_number")] public bool ShareNumber { get; set; }
}