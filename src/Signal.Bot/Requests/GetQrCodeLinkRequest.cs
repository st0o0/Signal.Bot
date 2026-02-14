using System.Text.Json.Serialization;

namespace Signal.Bot.Requests;

public record GetQrCodeLinkRequest() : RequestBase("v1/qrcodelink", HttpMethod.Get)
{
    [JsonPropertyName("device_name")] public required string DeviceName { get; set; }

    [JsonPropertyName("qrcode_version")] public int QrCodeVersion { get; set; } = 10;
}