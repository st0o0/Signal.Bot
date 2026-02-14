using System.Text.Json.Serialization;

namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to unregister the current device from Signal and optionally delete the account and local data.
/// </summary>
/// <param name="Number">The phone number of the Signal account to unregister.</param>
public record UnregisterDeviceRequest(string Number) : RequestBase($"v1/unregister/{Number}", HttpMethod.Delete)
{
    /// <summary>
    /// Gets or sets whether to permanently delete the Signal account. If <see langword="true"/>, the account cannot be recovered.
    /// </summary>
    [JsonPropertyName("delete_account")] 
    public bool DeleteAccount { get; set; }

    /// <summary>
    /// Gets or sets whether to delete local data stored by the bot, including messages and attachments.
    /// </summary>
    [JsonPropertyName("delete_local_data")] 
    public bool DeleteLocalData { get; set; }
}