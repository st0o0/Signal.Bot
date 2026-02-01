namespace Signal.Bot.Requests;

public record UpdateAccountSettingsRequest(string Number)
    : RequestBase($"v1/accounts/{Number}/settings", HttpMethod.Put)
{
    public bool DiscoverableByNumber { get; set; }
    public bool ShareNumberWithContacts { get; set; }
}