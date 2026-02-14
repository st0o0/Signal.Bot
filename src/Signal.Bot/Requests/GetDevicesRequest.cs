using Signal.Bot.Types;

namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to retrieve all linked devices associated with a Signal account.
/// </summary>
/// <param name="Number">The phone number of the Signal account whose devices should be retrieved.</param>
public record GetDevicesRequest(string Number)
    : RequestBase<List<Device>?>($"v1/devices/{Number}", HttpMethod.Get);