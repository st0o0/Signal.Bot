namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to retrieve the avatar image of a Signal group.
/// </summary>
/// <param name="Number">The phone number of the Signal account.</param>
/// <param name="GroupId">The unique identifier of the group.</param>
public record GetGroupAvatarRequest(string Number, string GroupId)
    : RequestBase($"v1/groups/{Number}/{GroupId}/avatar", HttpMethod.Get);
