namespace Signal.Bot.Requests;

public class GetAccountsRequest() : RequestBase<ICollection<string>>("v1/accounts")
{
    public override HttpMethod HttpMethod => HttpMethod.Get;
}