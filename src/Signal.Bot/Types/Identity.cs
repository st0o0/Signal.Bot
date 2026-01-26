namespace Signal.Bot.Types;

public class Identity
{
    [JsonPropertyName("added")] public string? Added { get; set; }

    [JsonPropertyName("fingerprint")] public string? Fingerprint { get; set; }

    [JsonPropertyName("number")] public string? Number { get; set; }

    [JsonPropertyName("safety_number")] public string? SafetyNumber { get; set; }

    [JsonPropertyName("status")] public string? Status { get; set; }

    [JsonPropertyName("uuid")] public string? Uuid { get; set; }
}