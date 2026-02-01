namespace Signal.Bot.Requests;

public record AddStickerPackRequest(string Number) : RequestBase($"v1/sticker-packs/{Number}")
{
    public string? PackId { get; set; }
    public string? PackKey { get; set; }
}