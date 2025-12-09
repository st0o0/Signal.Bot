namespace Signal.Bot.Requests;

public class RemoveReactionRequest(string number) : RequestBase<string>($"v1/reactions/{number}")
{
    [JsonIgnore] public string Number => number;
    public override HttpMethod HttpMethod => HttpMethod.Delete;

    public string? Emoji { get; set; }
    public string? Recipient { get; set; }
    public string? TargetAuthor { get; set; }
    public int? Timestamp { get; set; }
}