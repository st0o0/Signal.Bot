using Signal.Bot.Types;

namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to retrieve the current configuration settings of the Signal Bot API.
/// </summary>
public record GetConfigurationRequest() : RequestBase<Configuration>("v1/configuration", HttpMethod.Get);