namespace Signal.Bot.Requests;

public record GetAccountsRequest() : RequestBase<List<string>?>("v1/accounts", HttpMethod.Get);