using System.Text.Json;
using System.Text.Json.Serialization;
using Signal.Bot.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents a link preview with metadata and thumbnail for a URL mentioned in a message.
/// </summary>
public record PreviewData
{
    /// <summary>
    /// Gets or sets the URL being previewed.
    /// </summary>
    [JsonPropertyName("url")] 
    public string? Url { get; set; }

    /// <summary>
    /// Gets or sets the title of the linked page or resource.
    /// </summary>
    [JsonPropertyName("title")] 
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the description text extracted from the linked page.
    /// </summary>
    [JsonPropertyName("description")] 
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the thumbnail image attachment for the preview.
    /// </summary>
    [JsonPropertyName("image")] 
    public Attachment? Image { get; set; }
    
    /// <inheritdoc />
    public override string ToString() => JsonSerializer.Serialize(this, JsonBotAPI.Get(GetType()));
}