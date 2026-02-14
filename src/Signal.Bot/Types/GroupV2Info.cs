using System.Text.Json.Serialization;

namespace Signal.Bot.Types;

public class GroupV2Info
{
    [JsonPropertyName("id")] public string? Id { get; set; }

    [JsonPropertyName("name")] public string? Name { get; set; }

    [JsonPropertyName("revision")] public int? Revision { get; set; }
}