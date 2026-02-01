namespace Signal.Bot.Requests;

public record RemovePinRequest(string Number) : RequestBase($"v1/accounts/{Number}/pin", HttpMethod.Delete);