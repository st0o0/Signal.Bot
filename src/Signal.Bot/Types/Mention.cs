using System;

namespace Signal.Bot.Types;

public record Mention
{
    [JsonPropertyName("start")] public int? Start { get; set; }

    [JsonPropertyName("length")] public int? Length { get; set; }

    [JsonPropertyName("uuid")] public Guid Id { get; set; }
}