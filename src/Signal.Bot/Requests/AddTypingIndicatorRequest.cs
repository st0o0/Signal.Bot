using System.Text.Json.Serialization;

namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to display a typing indicator to show that the bot is composing a message.
/// </summary>
/// <param name="Number">The phone number of the Signal account sending the typing indicator.</param>
public record AddTypingIndicatorRequest(string Number) : RequestBase($"v1/typing-indicator/{Number}", HttpMethod.Put)
{
    /// <summary>
    /// Gets or sets the phone number or group ID of the recipient who will see the typing indicator.
    /// </summary>
    [JsonPropertyName("recipient")] 
    public string? Recipient { get; set; }
}