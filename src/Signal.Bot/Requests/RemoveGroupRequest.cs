namespace Signal.Bot.Requests;

public record RemoveGroupRequest(string Number, string GroupId) : RequestBase($"v1/groups/{Number}/{GroupId}",  HttpMethod.Delete);