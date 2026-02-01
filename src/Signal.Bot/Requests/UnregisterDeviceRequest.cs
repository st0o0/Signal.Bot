namespace Signal.Bot.Requests;

public record UnregisterDeviceRequest(string Number) : RequestBase($"v1/unregister/{Number}", HttpMethod.Delete)
{
    public bool DeleteAccount { get; set; }

    public bool DeleteLocalData { get; set; }
}