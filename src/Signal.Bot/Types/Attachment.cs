using System.Text.Json;
using System.Text.Json.Serialization;
using Signal.Bot.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents a file attachment in a Signal message.
/// </summary>
public record Attachment
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

    /// <summary>
    /// Gets or sets the width of the attachment in pixels, if applicable.
    /// </summary>
    [JsonPropertyName("width")]
    public int? Width { get; set; }

    /// <summary>
    /// Gets or sets the height of the attachment in pixels, if applicable.
    /// </summary>
    [JsonPropertyName("height")]
    public int? Height { get; set; }

    /// <summary>
    /// Gets or sets the caption text for this attachment.
    /// </summary>
    [JsonPropertyName("caption")]
    public string? Caption { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the attachment was uploaded.
    /// </summary>
    [JsonPropertyName("uploadTimestamp")]
    public DateTime? UploadTimestamp { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this attachment is a voice note.
    /// </summary>
    [JsonPropertyName("isVoiceNote")]
    public bool? IsVoiceNote { get; set; }

    /// <inheritdoc />
    public override string ToString() => JsonSerializer.Serialize(this, JsonBotAPI.Get(GetType()));
}