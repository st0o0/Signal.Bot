using Signal.Bot.Types;

namespace Signal.Bot.Requests;

public record RemoteDeleteRequest(string Number) : RequestBase<Acknowledged>($"v1/remote-delete/{Number}")
{
    [JsonPropertyName("recipient")] public string? Recipient { get; set; }
    
    [JsonPropertyName("timestamp")] public DateTime Timestamp { get; set; }
}