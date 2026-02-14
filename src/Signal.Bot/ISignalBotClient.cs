using System.Text.Json;
using R3;
using Signal.Bot.Args;
using Signal.Bot.Internal;
using Signal.Bot.Requests;

namespace Signal.Bot;

/// <summary>
/// Defines the core interface for interacting with the Signal Bot API, providing methods for sending requests and observing API interactions.
/// </summary>
public interface ISignalBotClient : IDisposable
{
    /// <summary>
    /// Gets the base URL of the Signal Bot API endpoint.
    /// </summary>
    string BaseUrl { get; }

    /// <summary>
    /// Gets the Signal phone number associated with this bot client.
    /// </summary>
    string Number { get; }

    /// <summary>
    /// Gets the JSON serialization options used for request and response serialization.
    /// </summary>
    JsonSerializerOptions JsonSerializerOptions { get; }

    /// <summary>
    /// Observable stream of outgoing API request events for monitoring and logging purposes.
    /// </summary>
    Observable<OnApiRequestArgs> OnApiRequest { get; }

    /// <summary>
    /// Observable stream of incoming API response events for monitoring and logging purposes.
    /// </summary>
    Observable<OnApiResponseArgs> OnApiResponse { get; }

    /// <summary>
    /// Observable stream of exceptions that occur during API interactions.
    /// </summary>
    Observable<Exception> OnException { get; }

    /// <summary>
    /// Sends an HTTP request to the Signal Bot API and returns the raw HTTP response.
    /// </summary>
    /// <param name="request">The request to send.</param>
    /// <param name="queryParameters">Optional query parameters to append to the request URL.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>The HTTP response message.</returns>
    Task<HttpResponseMessage> SendAsync(
        IRequest request,
        IQueryParameterRegistry? queryParameters = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a request to the Signal Bot API without expecting a response body.
    /// </summary>
    /// <param name="request">The request to send.</param>
    /// <param name="queryParameters">Optional query parameters to append to the request URL.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A task that completes when the request is sent successfully.</returns>
    Task SendRequestAsync(
        IRequest request,
        IQueryParameterRegistry? queryParameters = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a request to the Signal Bot API and deserializes the response to the specified type.
    /// </summary>
    /// <typeparam name="TResponse">The type to deserialize the response into.</typeparam>
    /// <param name="request">The request to send.</param>
    /// <param name="queryParameters">Optional query parameters to append to the request URL.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>The deserialized response of type TResponse.</returns>
    Task<TResponse> SendRequestAsync<TResponse>(
        IRequest<TResponse> request,
        IQueryParameterRegistry? queryParameters = null,
        CancellationToken cancellationToken = default
    );
}