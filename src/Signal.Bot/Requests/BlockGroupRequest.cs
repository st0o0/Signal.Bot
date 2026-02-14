namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to block all messages from a specific Signal group.
/// </summary>
/// <param name="Number">The phone number of the Signal account blocking the group.</param>
/// <param name="GroupId">The unique identifier of the group to block.</param>
public record BlockGroupRequest(string Number, string GroupId) : RequestBase($"/v1/groups/{Number}/{GroupId}/block");