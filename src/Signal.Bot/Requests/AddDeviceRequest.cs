namespace Signal.Bot.Requests;

public record AddDeviceRequest(string Number) : RequestBase($"v1/devices/{Number}")
{
    public string? Uri { get; set; }
}