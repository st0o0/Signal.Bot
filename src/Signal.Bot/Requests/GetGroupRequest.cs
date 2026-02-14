using Signal.Bot.Types;

namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to retrieve detailed information about a specific Signal group.
/// </summary>
/// <param name="Number">The phone number of the Signal account making the request.</param>
/// <param name="GroupId">The unique identifier of the group to retrieve.</param>
public record GetGroupRequest(string Number, string GroupId)
    : RequestBase<Group>($"v1/groups/{Number}/{GroupId}", HttpMethod.Get);