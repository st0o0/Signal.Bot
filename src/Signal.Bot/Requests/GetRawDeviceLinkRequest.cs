using Signal.Bot.Types;

namespace Signal.Bot.Requests;

public record GetRawDeviceLinkRequest() : RequestBase<RawDeviceLink>("/v1/qrcodelink/raw", HttpMethod.Get)
{
    [JsonPropertyName("device_name")] public required string DeviceName { get; set; }
}