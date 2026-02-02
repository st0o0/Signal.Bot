namespace Signal.Bot.Requests;

public record AddTypingIndicatorRequest(string Number) : RequestBase($"v1/typing-indicator/{Number}", HttpMethod.Put)
{
    [JsonPropertyName("recipient")] public string? Recipient { get; set; }
}