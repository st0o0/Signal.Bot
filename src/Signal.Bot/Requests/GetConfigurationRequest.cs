using Signal.Bot.Types;

namespace Signal.Bot.Requests;

public class GetConfigurationRequest() : RequestBase<Configuration>("v1/configuration")
{
    public override HttpMethod HttpMethod => HttpMethod.Get;
}