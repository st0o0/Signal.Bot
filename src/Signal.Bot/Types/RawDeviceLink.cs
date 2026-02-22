using System.Text.Json;
using System.Text.Json.Serialization;
using Signal.Bot.Serialization;

namespace Signal.Bot.Types;

/// <summary>
/// Represents the raw device linking URI used for provisioning a new device without a QR code.
/// </summary>
public record RawDeviceLink
{
    /// <summary>
    /// Gets or sets the device linking URI that can be used to link a new device programmatically.
    /// </summary>
    [JsonPropertyName("device_link_uri")] 
    public string? DeviceLinkUri { get; set; }
    
    /// <inheritdoc />
    public override string ToString() => JsonSerializer.Serialize(this, JsonBotAPI.Get(GetType()));
}