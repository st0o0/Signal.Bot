using System.Text.Json.Serialization;

namespace Signal.Bot.Types;

public class Mention
{
    [JsonPropertyName("start")] public int? Start { get; set; }

    [JsonPropertyName("length")] public int? Length { get; set; }

    [JsonPropertyName("uuid")] public Guid Id { get; set; }
}