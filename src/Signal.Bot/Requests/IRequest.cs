namespace Signal.Bot.Requests;

/// <summary>
/// Defines the base contract for all Signal Bot API requests without a typed response.
/// </summary>
public interface IRequest
{
    /// <summary>
    /// Gets the HTTP method to use for this request (e.g., GET, POST, PUT, DELETE).
    /// </summary>
    HttpMethod HttpMethod { get; }

    /// <summary>
    /// Gets the API endpoint method name or path (e.g., "v2/send", "v1/groups/{number}").
    /// </summary>
    string MethodName { get; }

    /// <summary>
    /// Converts the request object to HTTP content for transmission to the API.
    /// </summary>
    /// <returns>An <see cref="HttpContent"/> instance containing the serialized request body.</returns>
    HttpContent ToHttpContent();
}

public interface IRequest<TResponse> : IRequest;