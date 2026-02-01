namespace Signal.Bot.Requests;

public class UnregisterDeviceRequest(string number) : RequestBase($"v1/unregister/{number}")
{
    [JsonIgnore] public string Number => number;

    public override HttpMethod HttpMethod => HttpMethod.Delete;

    public bool DeleteAccount { get; set; }

    public bool DeleteLocalData { get; set; }
}