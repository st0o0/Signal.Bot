namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to join an existing Signal group using a group link or invitation.
/// </summary>
/// <param name="Number">The phone number of the Signal account joining the group.</param>
/// <param name="GroupId">The unique identifier of the group to join.</param>
public record JoinGroupRequest(string Number, string GroupId) : RequestBase($"/v1/groups/{Number}/{GroupId}/join");