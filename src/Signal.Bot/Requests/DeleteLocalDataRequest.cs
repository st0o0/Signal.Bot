using System.Text.Json.Serialization;

namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to delete all local data for a Signal account.
/// </summary>
/// <param name="Number">The phone number of the Signal account.</param>
public record DeleteLocalDataRequest(string Number)
    : RequestBase($"v1/devices/{Number}/local-data", HttpMethod.Delete)
{
    /// <summary>
    /// Gets or sets whether to ignore if the account is still registered.
    /// </summary>
    [JsonPropertyName("ignore_registered")]
    public bool IgnoreRegistered { get; set; }
}
