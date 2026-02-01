using System;

namespace Signal.Bot.Requests;

public record AddReactionRequest(string Number) : RequestBase($"v1/reactions/{Number}")
{
    public string? Reaction { get; set; }
    public string? Recipient { get; set; }
    public string? TargetAuthor { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}