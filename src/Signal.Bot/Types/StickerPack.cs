using System.Text.Json.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents a Signal sticker pack with its metadata and installation status.
/// </summary>
public class StickerPack
{
    /// <summary>
    /// Gets or sets the author or creator of the sticker pack.
    /// </summary>
    [JsonPropertyName("author")] 
    public string? Author { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this sticker pack is installed on the account.
    /// </summary>
    [JsonPropertyName("installed")] 
    public bool Installed { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the sticker pack.
    /// </summary>
    [JsonPropertyName("pack_id")] 
    public string? PackId { get; set; }

    /// <summary>
    /// Gets or sets the title or name of the sticker pack.
    /// </summary>
    [JsonPropertyName("title")] 
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the URL where the sticker pack can be viewed or downloaded.
    /// </summary>
    [JsonPropertyName("url")] 
    public string? Url { get; set; }
}