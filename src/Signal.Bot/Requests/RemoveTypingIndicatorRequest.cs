namespace Signal.Bot.Requests;

public record RemoveTypingIndicatorRequest(string Number)
    : RequestBase($"v1/typing-indicator/{Number}", HttpMethod.Delete)
{
    [JsonPropertyName("recipient")] public string? Recipient { get; set; }
}