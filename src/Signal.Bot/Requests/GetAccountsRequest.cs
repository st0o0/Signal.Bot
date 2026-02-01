namespace Signal.Bot.Requests;

public record GetAccountsRequest() : RequestBase<ICollection<string>?>("v1/accounts", HttpMethod.Get);