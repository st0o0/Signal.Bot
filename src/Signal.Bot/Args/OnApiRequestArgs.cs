using Signal.Bot.Requests;

namespace Signal.Bot.Args;

public record OnApiRequestArgs(
    IRequest Request,
    HttpRequestMessage RequestMessage);