using System.Text.Json.Serialization;

namespace Signal.Bot.Types;

public class StickerPack
{
    [JsonPropertyName("author")] public string? Author { get; set; }

    [JsonPropertyName("installed")] public bool Installed { get; set; }

    [JsonPropertyName("pack_id")] public string? PackId { get; set; }

    [JsonPropertyName("title")] public string? Title { get; set; }

    [JsonPropertyName("url")] public string? Url { get; set; }
}