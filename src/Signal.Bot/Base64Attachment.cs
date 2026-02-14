namespace Signal.Bot;

/// <summary>
/// Represents a base64-encoded attachment with support for various input formats including raw base64, bytes, files, and data URIs.
/// </summary>
public class Base64Attachment
{
    /// <summary>
    /// Gets the base64-encoded string representation of the attachment, optionally including data URI metadata.
    /// </summary>
    public string Value { get; }

    private Base64Attachment(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a Base64Attachment from a raw base64-encoded string.
    /// </summary>
    /// <param name="base64Data">The base64-encoded string.</param>
    /// <returns>A new Base64Attachment instance.</returns>
    public static Base64Attachment FromBase64(string base64Data)
        => new(base64Data);

    /// <summary>
    /// Creates a Base64Attachment from a byte array.
    /// </summary>
    /// <param name="bytes">The byte array to encode.</param>
    /// <returns>A new Base64Attachment instance.</returns>
    public static Base64Attachment FromBytes(byte[] bytes)
        => new(Convert.ToBase64String(bytes));

    /// <summary>
    /// Creates a Base64Attachment as a data URI from base64-encoded data and a MIME type.
    /// </summary>
    /// <param name="base64Data">The base64-encoded string.</param>
    /// <param name="mimeType">The MIME type of the data (e.g., "image/png").</param>
    /// <returns>A new Base64Attachment instance with data URI format.</returns>
    public static Base64Attachment FromDataUri(string base64Data, string mimeType)
        => new($"data:{mimeType};base64,{base64Data}");

    /// <summary>
    /// Creates a Base64Attachment as a data URI from a byte array and a MIME type.
    /// </summary>
    /// <param name="bytes">The byte array to encode.</param>
    /// <param name="mimeType">The MIME type of the data (e.g., "image/png").</param>
    /// <returns>A new Base64Attachment instance with data URI format.</returns>
    public static Base64Attachment FromDataUri(byte[] bytes, string mimeType)
        => new($"data:{mimeType};base64,{Convert.ToBase64String(bytes)}");

    /// <summary>
    /// Creates a Base64Attachment as a data URI from base64-encoded data, MIME type, and filename.
    /// </summary>
    /// <param name="base64Data">The base64-encoded string.</param>
    /// <param name="mimeType">The MIME type of the data (e.g., "image/png").</param>
    /// <param name="filename">The filename to include in the data URI.</param>
    /// <returns>A new Base64Attachment instance with data URI format including filename.</returns>
    public static Base64Attachment FromDataUri(string base64Data, string mimeType, string filename)
        => new($"data:{mimeType};filename={filename};base64,{base64Data}");

    /// <summary>
    /// Creates a Base64Attachment as a data URI from a byte array, MIME type, and filename.
    /// </summary>
    /// <param name="bytes">The byte array to encode.</param>
    /// <param name="mimeType">The MIME type of the data (e.g., "image/png").</param>
    /// <param name="filename">The filename to include in the data URI.</param>
    /// <returns>A new Base64Attachment instance with data URI format including filename.</returns>
    public static Base64Attachment FromDataUri(byte[] bytes, string mimeType, string filename)
        => new($"data:{mimeType};filename={filename};base64,{Convert.ToBase64String(bytes)}");

    /// <summary>
    /// Creates a Base64Attachment from a file path, automatically detecting the MIME type from the file extension.
    /// </summary>
    /// <param name="filePath">The path to the file to encode.</param>
    /// <param name="mimeType">Optional MIME type override. If null, the MIME type is inferred from the file extension.</param>
    /// <param name="includeFilename">If true, includes the filename in the data URI.</param>
    /// <returns>A new Base64Attachment instance with data URI format.</returns>
    public static Base64Attachment FromFile(string filePath, string? mimeType = null, bool includeFilename = false)
    {
        var bytes = File.ReadAllBytes(filePath);
        var mime = mimeType ?? GetMimeType(filePath);
        var filename = Path.GetFileName(filePath);

        return includeFilename ? FromDataUri(bytes, mime, filename) : FromDataUri(bytes, mime);
    }

    /// <summary>
    /// Implicitly converts a Base64Attachment to its string representation.
    /// </summary>
    /// <param name="attachment">The attachment to convert.</param>
    public static implicit operator string(Base64Attachment attachment) => attachment.Value;

    /// <summary>
    /// Returns the base64-encoded string representation of the attachment.
    /// </summary>
    /// <returns>The Value property.</returns>
    public override string ToString() => Value;

    private static string GetMimeType(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".pdf" => "application/pdf",
            ".mp4" => "video/mp4",
            ".mp3" => "audio/mpeg",
            _ => "application/octet-stream"
        };
    }
}