namespace Signal.Bot.Requests;

public record VerifyNumberRequest(string Number, string Token)
    : RequestBase<string>($"v1/register/{Number}/verify/{Token}")
{
    [JsonPropertyName("pin")] public string? Pin { get; set; }
}