using System.Text.Json.Serialization;

namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to remove administrator privileges from one or more group members.
/// </summary>
/// <param name="Number">The phone number of the Signal account making the request.</param>
/// <param name="GroupId">The unique identifier of the group.</param>
public record RemoveGroupAdminRequest(string Number, string GroupId)
    : RequestBase($"v1/groups/{Number}/{GroupId}/admins", HttpMethod.Delete)
{
    /// <summary>
    /// Gets or sets the array of phone numbers of administrators to demote to regular members.
    /// </summary>
    [JsonPropertyName("admins")]
    public string[]? Admins { get; set; }
}