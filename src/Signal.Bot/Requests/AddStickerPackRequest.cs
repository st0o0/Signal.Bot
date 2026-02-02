namespace Signal.Bot.Requests;

public record AddStickerPackRequest(string Number) : RequestBase($"v1/sticker-packs/{Number}")
{
    [JsonPropertyName("pack_id")] public string? PackId { get; set; }
    
    [JsonPropertyName("pack_key")] public string? PackKey { get; set; }
}