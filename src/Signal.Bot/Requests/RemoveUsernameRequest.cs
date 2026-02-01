namespace Signal.Bot.Requests;

public record RemoveUsernameRequest(string Number)
    : RequestBase<object>($"v1/accounts/{Number}/username", HttpMethod.Delete);