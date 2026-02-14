namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to retrieve all registered Signal accounts managed by this bot instance.
/// </summary>
public record GetAccountsRequest() : RequestBase<List<string>?>("v1/accounts", HttpMethod.Get);