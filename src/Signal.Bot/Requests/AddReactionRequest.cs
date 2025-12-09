namespace Signal.Bot.Requests;

public class AddReactionRequest(string number) : RequestBase($"v1/reactions/{number}")
{
    [JsonIgnore] public string Number => number;

    public string? Reaction { get; set; }
    public string? Recipient { get; set; }
    public string? TargetAuthor { get; set; }
    public int? Timestamp { get; set; }
}