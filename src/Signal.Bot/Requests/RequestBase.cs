using System.Net.Http.Json;
using System.Text.Json;

namespace Signal.Bot.Requests;

public abstract class RequestBase(string methodName) : IRequest
{
    [JsonIgnore] public virtual HttpMethod HttpMethod { get; } = HttpMethod.Post;

    [JsonIgnore] public string MethodName { get; } = methodName;

    public virtual HttpContent? ToHttpContent() => JsonContent.Create(this, GetType(),
        options: new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
        });
}

public abstract class RequestBase<TResponse>(string methodName) : IRequest<TResponse>
{
    [JsonIgnore] public virtual HttpMethod HttpMethod { get; } = HttpMethod.Post;

    [JsonIgnore] public string MethodName { get; } = methodName;

    public virtual HttpContent? ToHttpContent() => JsonContent.Create(this, GetType(),
        options: new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
        });
}