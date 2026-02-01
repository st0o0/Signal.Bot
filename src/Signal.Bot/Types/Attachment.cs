namespace Signal.Bot.Types;

public class Attachment
{
    [JsonPropertyName("id")] public string? Id { get; set; }

    [JsonPropertyName("filename")] public string? Filename { get; set; }

    [JsonPropertyName("contentType")] public string? ContentType { get; set; }

    [JsonPropertyName("size")] public long? Size { get; set; }
}