using Signal.Bot.Types;

namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to retrieve all Signal groups that the account is a member of.
/// </summary>
/// <param name="Number">The phone number of the Signal account whose groups should be retrieved.</param>
public record GetGroupsRequest(string Number) : RequestBase<List<Group>?>($"v1/groups/{Number}",  HttpMethod.Get);