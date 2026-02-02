namespace Signal.Bot.Requests;

public record SetPinRequest(string Number) : RequestBase($"v1/accounts/{Number}/pin")
{
    [JsonPropertyName("pin")] public string? Pin { get; set; }
}