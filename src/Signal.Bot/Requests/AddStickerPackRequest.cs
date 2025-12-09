namespace Signal.Bot.Requests;

public class AddStickerPackRequest(string number) : RequestBase($"v1/sticker-packs/{number}")
{
    [JsonIgnore] public string Number => number;
    public string? PackId { get; set; }
    public string? PackKey { get; set; }
}