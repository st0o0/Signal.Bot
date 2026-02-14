using System.Text.Json.Serialization;

namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to add a new device to a Signal account.
/// </summary>
/// <param name="Number">The phone number of the Signal account to which the device will be added.</param>
public record AddDeviceRequest(string Number) : RequestBase($"v1/devices/{Number}")
{
    /// <summary>
    /// Gets or sets the device URI/link for pairing the new device (optional).
    /// </summary>
    [JsonPropertyName("uri")] public string? Uri { get; set; }
}