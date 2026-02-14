using System.Text.Json.Serialization;
using Signal.Bot.Types;

namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to retrieve raw device linking information for provisioning a new device without a QR code.
/// </summary>
public record GetRawDeviceLinkRequest() : RequestBase<RawDeviceLink>("/v1/qrcodelink/raw", HttpMethod.Get)
{
    /// <summary>
    /// Gets or sets the name to assign to the new device being linked.
    /// </summary>
    [JsonPropertyName("device_name")] 
    public required string DeviceName { get; set; }
}