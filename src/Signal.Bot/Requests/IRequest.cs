namespace Signal.Bot.Requests;

public interface IRequest
{
    HttpMethod HttpMethod { get; }
    string MethodName { get; }
    HttpContent? ToHttpContent();
}

public interface IRequest<TResponse> : IRequest;