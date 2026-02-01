using Signal.Bot.Types;

namespace Signal.Bot.Requests;

public record GetStickerPacksRequest(string Number)
    : RequestBase<ICollection<StickerPack>?>($"v1/sticker-packs/{Number}", HttpMethod.Get);