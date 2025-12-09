namespace Signal.Bot.Types;

public class Identity
{
    [JsonPropertyName("added")] public string? Added { get; set; } = null;

    [JsonPropertyName("fingerprint")] public string? Fingerprint { get; set; } = null;

    [JsonPropertyName("number")] public string? Number { get; set; } = null;

    [JsonPropertyName("safety_number")] public string? SafetyNumber { get; set; } = null;

    [JsonPropertyName("status")] public string? Status { get; set; } = null;

    [JsonPropertyName("uuid")] public string? Uuid { get; set; } = null;
}