using System.Text.Json.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents a file attachment in a Signal message.
/// </summary>
public class Attachment
{
    /// <summary>
    /// Gets or sets the unique identifier of the attachment.
    /// </summary>
    [JsonPropertyName("id")] 
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the filename of the attachment.
    /// </summary>
    [JsonPropertyName("filename")] 
    public string? Filename { get; set; }

    /// <summary>
    /// Gets or sets the MIME type of the attachment (e.g., "image/jpeg", "application/pdf").
    /// </summary>
    [JsonPropertyName("contentType")] 
    public string? ContentType { get; set; }

    /// <summary>
    /// Gets or sets the size of the attachment in bytes.
    /// </summary>
    [JsonPropertyName("size")] 
    public long? Size { get; set; }
}