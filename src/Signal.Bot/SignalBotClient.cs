using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Signal.Bot.Args;
using Signal.Bot.Exceptions;
using Signal.Bot.Requests;
using Signal.Bot.Serialization;
using R3;

namespace Signal.Bot;

/// <summary>
/// Defines the core interface for interacting with the Signal Bot API, providing methods for sending requests and observing API interactions.
/// </summary>
public sealed class SignalBotClient : ISignalBotClient
{
    private readonly HttpClient _httpClient;
    private readonly SignalBotClientOptions _options;
    private readonly Subject<OnApiRequestArgs> _onApiRequest = new();
    private readonly Subject<OnApiResponseArgs> _onApiResponse = new();
    private readonly Subject<Exception> _onException = new();

    /// <summary>
    /// Initializes a new instance of the SignalBotClient with optional configuration.
    /// </summary>
    /// <param name="configure">Optional action to configure the client options via a builder.</param>
    public SignalBotClient(Action<SignalBotClientOptionsBuilder>? configure = null)
    {
        var builder = SignalBotClientOptionsBuilder.Create();
        configure?.Invoke(builder);
        _options = builder.Build();
        _httpClient = _options.HttpClient!;
        JsonSerializerOptions = JsonBotAPI.Options;
    }

    /// <summary>
    /// Gets the base URL of the Signal Bot API endpoint.
    /// </summary>
    public string BaseUrl => _options.BaseUrl;

    /// <summary>
    /// Gets the Signal phone number associated with this bot client.
    /// </summary>
    public string Number => _options.Number;

    /// <summary>
    /// Gets the JSON serialization options used for request and response serialization.
    /// </summary>
    public JsonSerializerOptions JsonSerializerOptions { get; }

    /// <summary>
    /// Observable stream of outgoing API request events for monitoring and logging purposes.
    /// </summary>
    public Observable<OnApiRequestArgs> OnApiRequest => _onApiRequest.AsObservable();

    /// <summary>
    /// Observable stream of incoming API response events for monitoring and logging purposes.
    /// </summary>
    public Observable<OnApiResponseArgs> OnApiResponse => _onApiResponse.AsObservable();

    /// <summary>
    /// Observable stream of exceptions that occur during API interactions.
    /// </summary>
    public Observable<Exception> OnException => _onException.AsObservable();

    /// <summary>
    /// Sends an HTTP request to the Signal Bot API and returns the raw HTTP response.
    /// </summary>
    /// <param name="request">The request to send.</param>
    /// <param name="queryParameters">Optional query parameters to append to the request URL.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>The HTTP response message.</returns>
    public async Task<HttpResponseMessage> SendAsync(IRequest request,
        IQueryParameterRegistry? queryParameters = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var methodName = request.MethodName;

            var httpRequest = new HttpRequestMessage(request.HttpMethod, methodName)
            {
                Content = request.ToHttpContent()
            };
            _onApiRequest.OnNext(new OnApiRequestArgs(request, httpRequest));
            HttpResponseMessage? httpResponse;
            try
            {
                httpResponse = await _httpClient
                    .SendAsync(httpRequest, cancellationToken)
                    .ConfigureAwait(false);

                if (httpResponse.StatusCode is HttpStatusCode.BadRequest)
                {
                    var content = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
                    var error = JsonSerializer.Deserialize(content, JsonBotAPI.Get<Types.ErrorResponse>())!;
                    throw new HttpRequestException(error.Message);
                }

                httpResponse.EnsureSuccessStatusCode();
            }
            catch (TaskCanceledException exception)
            {
                if (cancellationToken.IsCancellationRequested) throw;
                throw new RequestException("Bot API Request timed out", exception);
            }

            _onApiResponse.OnNext(new OnApiResponseArgs(request, httpRequest, httpResponse));
            return httpResponse;
        }
        catch (Exception ex)
        {
            _onException.OnNext(ex);
            _onApiRequest.OnErrorResume(ex);
            _onApiResponse.OnErrorResume(ex);
            return null!;
        }
    }

    /// <summary>
    /// Sends a request to the Signal Bot API without expecting a response body.
    /// </summary>
    /// <param name="request">The request to send.</param>
    /// <param name="queryParameters">Optional query parameters to append to the request URL.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A task that completes when the request is sent successfully.</returns>
    public async Task SendRequestAsync(
        IRequest request,
        IQueryParameterRegistry? queryParameters = null,
        CancellationToken cancellationToken = default)
    {
        _ = await SendAsync(request, queryParameters, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a request to the Signal Bot API and deserializes the response to the specified type.
    /// </summary>
    /// <typeparam name="TResponse">The type to deserialize the response into.</typeparam>
    /// <param name="request">The request to send.</param>
    /// <param name="queryParameters">Optional query parameters to append to the request URL.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>The deserialized response of type TResponse.</returns>
    public async Task<TResponse> SendRequestAsync<TResponse>(IRequest<TResponse> request,
        IQueryParameterRegistry? queryParameters = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var httpResponse = await SendAsync(request, queryParameters, cancellationToken: cancellationToken);
            var response = await httpResponse.Content.ReadFromJsonAsync(JsonBotAPI.Get<TResponse>(), cancellationToken);
            return response!;
        }
        catch (Exception ex)
        {
            _onException.OnNext(ex);
            _onApiRequest.OnErrorResume(ex);
            _onApiResponse.OnErrorResume(ex);
            return default!;
        }
    }

    /// <summary>
    /// Releases the unmanaged resources used by the SignalBotClient and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    private void Dispose(bool disposing)
    {
        if (!disposing) return;

        _httpClient.Dispose();
        _onApiRequest.OnCompleted();
        _onApiResponse.OnCompleted();
        _onException.OnCompleted();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}