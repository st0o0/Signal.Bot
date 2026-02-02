namespace Signal.Bot.Requests;

public record UnregisterDeviceRequest(string Number) : RequestBase($"v1/unregister/{Number}", HttpMethod.Delete)
{
    [JsonPropertyName("delete_account")] public bool DeleteAccount { get; set; }

    [JsonPropertyName("delete_local_data")] public bool DeleteLocalData { get; set; }
}