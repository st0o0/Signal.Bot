using System;
using System.IO;

namespace Signal.Bot;

public class Base64Attachment
{
    public string Value { get; }

    private Base64Attachment(string value)
    {
        Value = value;
    }

    public static Base64Attachment FromBase64(string base64Data)
        => new(base64Data);

    public static Base64Attachment FromBytes(byte[] bytes)
        => new(Convert.ToBase64String(bytes));

    public static Base64Attachment FromDataUri(string base64Data, string mimeType)
        => new($"data:{mimeType};base64,{base64Data}");

    public static Base64Attachment FromDataUri(byte[] bytes, string mimeType)
        => new($"data:{mimeType};base64,{Convert.ToBase64String(bytes)}");

    public static Base64Attachment FromDataUri(string base64Data, string mimeType, string filename)
        => new($"data:{mimeType};filename={filename};base64,{base64Data}");

    public static Base64Attachment FromDataUri(byte[] bytes, string mimeType, string filename)
        => new($"data:{mimeType};filename={filename};base64,{Convert.ToBase64String(bytes)}");

    public static Base64Attachment FromFile(string filePath, string? mimeType = null, bool includeFilename = false)
    {
        var bytes = File.ReadAllBytes(filePath);
        var mime = mimeType ?? GetMimeType(filePath);
        var filename = Path.GetFileName(filePath);

        return includeFilename ? FromDataUri(bytes, mime, filename) : FromDataUri(bytes, mime);
    }

    public static implicit operator string(Base64Attachment attachment) => attachment.Value;

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