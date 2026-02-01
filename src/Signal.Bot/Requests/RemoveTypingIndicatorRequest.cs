namespace Signal.Bot.Requests;

public record SetTypingIndicatorRequest(string Number) : RequestBase($"v1/typing-indicator/{Number}", HttpMethod.Put)
{
    public string? Recipient { get; set; }

    public string? GroupId { get; set; }
}

public record RemoveTypingIndicatorRequest(string Number)
    : RequestBase($"v1/typing-indicator/{Number}", HttpMethod.Delete)
{
    public string? Recipient { get; set; }

    public string? GroupId { get; set; }
}