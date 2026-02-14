using System.Text.Json.Serialization;

namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to remove one or more members from a Signal group.
/// </summary>
/// <param name="Number">The phone number of the Signal account making the request.</param>
/// <param name="GroupId">The unique identifier of the group.</param>
public record RemoveGroupMemberRequest(string Number, string GroupId)
    : RequestBase($"v1/groups/{Number}/{GroupId}/members", HttpMethod.Delete)
{
    /// <summary>
    /// Gets or sets the array of phone numbers of members to remove from the group.
    /// </summary>
    [JsonPropertyName("members")] 
    public string[]? Members { get; set; }
}