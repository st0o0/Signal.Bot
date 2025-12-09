namespace Signal.Bot.Requests;

public class AddDeviceRequest(string number) : RequestBase($"v1/devices/{number}")
{
    [JsonIgnore] public string Number => number;
    public string? Uri { get; set; }
}
