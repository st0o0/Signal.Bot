using Signal.Bot.Types;

namespace Signal.Bot.Requests;

public class GetAboutRequest() : RequestBase<About>("v1/about")
{
    public override HttpMethod HttpMethod => HttpMethod.Get;
}