namespace Signal.Bot.Requests;

public record JoinGroupRequest(string Number, string GroupId) : RequestBase($"/v1/groups/{Number}/{GroupId}/join");