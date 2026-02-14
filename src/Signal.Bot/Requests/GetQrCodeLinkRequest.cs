using System.Text.Json.Serialization;

namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to generate a QR code link for linking a new device to the Signal account.
/// </summary>
public record GetQrCodeLinkRequest() : RequestBase("v1/qrcodelink", HttpMethod.Get)
{
    /// <summary>
    /// Gets or sets the name to assign to the new device being linked.
    /// </summary>
    [JsonPropertyName("device_name")] 
    public required string DeviceName { get; set; }

    /// <summary>
    /// Gets or sets the QR code version to generate. Default is 10.
    /// </summary>
    /// <value>
    /// The QR code version number, typically between 1 and 40. Higher versions can store more data.
    /// </value>
    [JsonPropertyName("qrcode_version")] 
    public int QrCodeVersion { get; set; } = 10;
}