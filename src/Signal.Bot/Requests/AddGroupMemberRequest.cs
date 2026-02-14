using System.Text.Json.Serialization;

namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to add members to a Signal group.
/// </summary>
/// <param name="Number">The phone number of the account making the request.</param>
/// <param name="GroupId">The ID of the group to which members will be added.</param>
public record AddGroupMemberRequest(string Number, string GroupId)
    : RequestBase($"v1/groups/{Number}/{GroupId}/members")
{
    /// <summary>
    /// Gets or sets the collection of phone numbers to be added as group members.
    /// </summary>
    [JsonPropertyName("members")] public string[]? Members { get; set; }
}