using System.Net.Http.Json;
using Signal.Bot.Serialization;

namespace Signal.Bot.Requests;

public abstract record RequestBase(string MethodName, HttpMethod? Method = null) : IRequest
{
    public HttpMethod HttpMethod => Method ?? HttpMethod.Post;
    public virtual HttpContent ToHttpContent() => JsonContent.Create(this, GetType(), options: JsonBotAPI.Options);
}

public abstract record RequestBase<TResponse>(string MethodName, HttpMethod? Method = null)
    : RequestBase(MethodName, Method), IRequest<TResponse>;