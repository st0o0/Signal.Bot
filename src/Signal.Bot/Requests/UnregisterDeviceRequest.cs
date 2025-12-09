namespace Signal.Bot.Requests;

public class UnregisterDeviceRequest(string number) : RequestBase($"v1/unregister/{number}")
{
    [JsonIgnore] public string Number => number;

    public bool DeleteAccount { get; set; }

    public bool DeleteLocalData { get; set; }

    public override HttpMethod HttpMethod => HttpMethod.Delete;
}