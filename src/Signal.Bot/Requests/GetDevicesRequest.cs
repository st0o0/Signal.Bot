using Signal.Bot.Types;

namespace Signal.Bot.Requests;

public record GetDevicesRequest(string Number)
    : RequestBase<ICollection<Device>?>($"v1/devices/{Number}", HttpMethod.Get);