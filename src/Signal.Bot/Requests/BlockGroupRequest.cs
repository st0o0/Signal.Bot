namespace Signal.Bot.Requests;

public record BlockGroupRequest(string Number, string GroupId) : RequestBase($"/v1/groups/{Number}/{GroupId}/block");