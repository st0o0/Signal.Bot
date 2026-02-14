using System.Net.Http.Json;
using Signal.Bot.Serialization;

namespace Signal.Bot.Requests;

/// <summary>
/// Base class for all Signal Bot API requests without a typed response.
/// Provides common functionality for HTTP method specification and JSON serialization.
/// </summary>
/// <param name="MethodName">The API endpoint method name (e.g., "v2/send", "v1/groups/{number}").</param>
/// <param name="Method">Optional HTTP method to use. Defaults to POST if not specified.</param>
public abstract record RequestBase(string MethodName, HttpMethod? Method = null) : IRequest
{
    /// <summary>
    /// Gets the HTTP method to use for this request. Defaults to POST if not specified in the constructor.
    /// </summary>
    public HttpMethod HttpMethod => Method ?? HttpMethod.Post;
    /// <summary>
    /// Converts the request object to HTTP content using JSON serialization with Signal Bot API options.
    /// </summary>
    /// <returns>An <see cref="HttpContent"/> instance containing the JSON-serialized request body.</returns>
    /// <seealso cref="JsonBotAPI.Options"/>
    public virtual HttpContent ToHttpContent() => JsonContent.Create(this, JsonBotAPI.Get(GetType()));
}

public abstract record RequestBase<TResponse>(string MethodName, HttpMethod? Method = null)
    : RequestBase(MethodName, Method), IRequest<TResponse>;