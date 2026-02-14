using System.Text.Json.Serialization;

namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to install a sticker pack to the Signal account.
/// </summary>
/// <param name="Number">The phone number of the Signal account installing the sticker pack.</param>
public record AddStickerPackRequest(string Number) : RequestBase($"v1/sticker-packs/{Number}")
{
    /// <summary>
    /// Gets or sets the unique identifier of the sticker pack to install.
    /// </summary>
    [JsonPropertyName("pack_id")] 
    public string? PackId { get; set; }
    
    /// <summary>
    /// Gets or sets the decryption key for the sticker pack.
    /// </summary>
    [JsonPropertyName("pack_key")] 
    public string? PackKey { get; set; }
}