using System.Text.Json.Serialization;

namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to stop displaying the typing indicator, signaling that message composition has stopped.
/// </summary>
/// <param name="Number">The phone number of the Signal account removing the typing indicator.</param>
public record RemoveTypingIndicatorRequest(string Number)
    : RequestBase($"v1/typing-indicator/{Number}", HttpMethod.Delete)
{
    /// <summary>
    /// Gets or sets the phone number or group ID of the recipient who will no longer see the typing indicator.
    /// </summary>
    [JsonPropertyName("recipient")] 
    public string? Recipient { get; set; }
}