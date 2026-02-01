using Signal.Bot.Types;

namespace Signal.Bot.Requests;

public record GetConfigurationRequest() : RequestBase<Configuration>("v1/configuration", HttpMethod.Get);