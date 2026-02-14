using System.Text.Json.Serialization;
using Signal.Bot.Types;

namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to remotely delete a message from a conversation for all participants.
/// </summary>
/// <param name="Number">The phone number of the Signal account deleting the message.</param>
public record RemoteDeleteRequest(string Number) : RequestBase<Acknowledged>($"v1/remote-delete/{Number}")
{
    /// <summary>
    /// Gets or sets the phone number or group ID of the conversation where the message should be deleted.
    /// </summary>
    [JsonPropertyName("recipient")] 
    public string? Recipient { get; set; }
    
    /// <summary>
    /// Gets or sets the timestamp of the message to delete.
    /// </summary>
    [JsonPropertyName("timestamp")] 
    public DateTime Timestamp { get; set; }
}