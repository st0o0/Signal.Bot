namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to remove a specific linked device from a Signal account.
/// </summary>
/// <param name="Number">The phone number of the Signal account.</param>
/// <param name="DeviceId">The ID of the device to remove.</param>
public record RemoveDeviceRequest(string Number, int DeviceId)
    : RequestBase($"v1/devices/{Number}/{DeviceId}", HttpMethod.Delete);
