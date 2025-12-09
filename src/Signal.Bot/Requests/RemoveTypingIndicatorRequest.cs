namespace Signal.Bot.Requests;

public class SetTypingIndicatorRequest(string number) : RequestBase($"v1/typing-indicator/{number}")
{
    [JsonIgnore] public string Number => number;
    public string? Recipient { get; set; }
    public string? GroupId { get; set; }
    public override HttpMethod HttpMethod => HttpMethod.Put;
}

public class RemoveTypingIndicatorRequest(string number) : RequestBase($"v1/typing-indicator/{number}")
{
    [JsonIgnore] public string Number => number;
    public string? Recipient { get; set; }
    public string? GroupId { get; set; }
    public override HttpMethod HttpMethod => HttpMethod.Delete;
}