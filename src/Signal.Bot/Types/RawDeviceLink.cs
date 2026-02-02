namespace Signal.Bot.Types;

public class RawDeviceLink
{
    [JsonPropertyName("device_link_uri")] public string? DeviceLinkUri { get; set; }
}