using Signal.Bot.Types;

namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to retrieve all sticker packs installed or available to the Signal account.
/// </summary>
/// <param name="Number">The phone number of the Signal account whose sticker packs should be retrieved.</param>
public record GetStickerPacksRequest(string Number)
    : RequestBase<List<StickerPack>?>($"v1/sticker-packs/{Number}", HttpMethod.Get);