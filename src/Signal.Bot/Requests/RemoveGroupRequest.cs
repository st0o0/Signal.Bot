namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to delete a Signal group. Only group administrators can perform this action.
/// </summary>
/// <param name="Number">The phone number of the Signal account deleting the group.</param>
/// <param name="GroupId">The unique identifier of the group to delete.</param>
public record RemoveGroupRequest(string Number, string GroupId) : RequestBase($"v1/groups/{Number}/{GroupId}",  HttpMethod.Delete);