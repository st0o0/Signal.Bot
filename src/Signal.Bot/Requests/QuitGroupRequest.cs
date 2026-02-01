namespace Signal.Bot.Requests;

public record QuitGroupRequest(string Number, string GroupId) : RequestBase($"v1/groups/{Number}/{GroupId}/quit");