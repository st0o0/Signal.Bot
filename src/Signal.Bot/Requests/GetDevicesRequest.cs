using Signal.Bot.Types;

namespace Signal.Bot.Requests;

public class GetDevicesRequest(string number) : RequestBase<ICollection<Device>>($"v1/devices/{number}")
{
    [JsonIgnore] public string Number => number;
    public override HttpMethod HttpMethod => HttpMethod.Get;
}
