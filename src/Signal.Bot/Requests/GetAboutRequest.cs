using Signal.Bot.Types;

namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to retrieve general information about the Signal Bot API service.
/// </summary>
public record GetAboutRequest() : RequestBase<About>("v1/about", HttpMethod.Get);