using System.Text.Json.Serialization;
using Signal.Bot.Types;

namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to set a unique username for the Signal account, enabling discovery without sharing the phone number.
/// </summary>
/// <param name="Number">The phone number of the Signal account for which the username is being set.</param>
public record SetUsernameRequest(string Number) : RequestBase<SetUsername>($"v1/accounts/{Number}/username")
{
    /// <summary>
    /// Gets or sets the username to set. Must be unique across all Signal users.
    /// </summary>
    [JsonPropertyName("username")] 
    public string? Username { get; set; }
}