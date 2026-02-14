namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to leave a Signal group, removing the account from the member list.
/// </summary>
/// <param name="Number">The phone number of the Signal account leaving the group.</param>
/// <param name="GroupId">The unique identifier of the group to leave.</param>
public record QuitGroupRequest(string Number, string GroupId) : RequestBase($"v1/groups/{Number}/{GroupId}/quit");