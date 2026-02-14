using System.Text.Json.Serialization;

namespace Signal.Bot.Types;

public class ReceivedMessage
{
    [JsonPropertyName("envelope")] public Envelope? Envelope { get; set; }

    [JsonPropertyName("account")] public string? Account { get; set; }
}