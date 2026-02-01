using Signal.Bot.Types;

namespace Signal.Bot.Requests;

public record GetGroupRequest(string Number, string GroupId)
    : RequestBase<Group>($"v1/groups/{Number}/{GroupId}", HttpMethod.Get);