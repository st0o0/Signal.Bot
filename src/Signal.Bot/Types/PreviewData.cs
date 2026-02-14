using System.Text.Json.Serialization;

namespace Signal.Bot.Types;

public class PreviewData
{
    [JsonPropertyName("url")] public string? Url { get; set; }

    [JsonPropertyName("title")] public string? Title { get; set; }

    [JsonPropertyName("description")] public string? Description { get; set; }

    [JsonPropertyName("image")] public Attachment? Image { get; set; }
}