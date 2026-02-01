using System;

namespace Signal.Bot.Requests;

public record RemoveReactionRequest(string Number) : RequestBase<string>($"v1/reactions/{Number}", HttpMethod.Delete)
{
    public string? Emoji { get; set; }
    public string? Recipient { get; set; }
    public string? TargetAuthor { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}