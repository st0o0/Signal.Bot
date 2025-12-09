using Signal.Bot.Requests;

namespace Signal.Bot.Args;

public record OnApiResponseArgs(
    IRequest Request,
    HttpRequestMessage RequestMessage,
    HttpResponseMessage ResponseMessage);