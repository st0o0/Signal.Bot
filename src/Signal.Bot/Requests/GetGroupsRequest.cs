using Signal.Bot.Types;

namespace Signal.Bot.Requests;

public record GetGroupsRequest(string Number) : RequestBase<List<Group>?>($"v1/groups/{Number}",  HttpMethod.Get);