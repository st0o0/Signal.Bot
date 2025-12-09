namespace Signal.Bot.Requests;

public class GetStickerPacksRequest(string number)
    : RequestBase<ICollection<InstalledStickerPack>>($"v1/sticker-packs/{number}")
{
    [JsonIgnore] public string Number => number;
    public override HttpMethod HttpMethod => HttpMethod.Get;
}

public class InstalledStickerPack
{
    [JsonPropertyName("author")] public string? Author { get; set; } = null;

    [JsonPropertyName("installed")] public bool? Installed { get; set; } = null;

    [JsonPropertyName("pack_id")] public string? PackId { get; set; } = null;

    [JsonPropertyName("title")] public string? Title { get; set; } = null;

    [JsonPropertyName("url")] public string? Url { get; set; } = null;
}