using Signal.Bot.Types;

namespace Signal.Bot.Requests;

public record GetGroupsRequest(string Number) : RequestBase<ICollection<Group>?>($"v1/groups/{Number}",  HttpMethod.Get);