using Signal.Bot.Types;

namespace Signal.Bot.Requests;

public record GetAboutRequest() : RequestBase<About>("v1/about", HttpMethod.Get);