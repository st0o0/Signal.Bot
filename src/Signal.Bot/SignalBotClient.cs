using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Signal.Bot.Args;
using Signal.Bot.Exceptions;
using Signal.Bot.Internal;
using Signal.Bot.Requests;
using Signal.Bot.Serialization;
using R3;

namespace Signal.Bot;

/// <inheritdoc />
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

    /// <inheritdoc />
    public string BaseUrl => _options.BaseUrl;

    /// <inheritdoc />
    public string Number => _options.Number;

    /// <inheritdoc />
    public JsonSerializerOptions JsonSerializerOptions { get; }

    /// <inheritdoc />
    public Observable<OnApiRequestArgs> OnApiRequest => _onApiRequest.AsObservable();

    /// <inheritdoc />
    public Observable<OnApiResponseArgs> OnApiResponse => _onApiResponse.AsObservable();

    /// <inheritdoc />
    public Observable<Exception> OnException => _onException.AsObservable();

    /// <inheritdoc />
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

    /// <inheritdoc />
    public async Task SendRequestAsync(
        IRequest request,
        IQueryParameterRegistry? queryParameters = null,
        CancellationToken cancellationToken = default)
    {
        _ = await SendAsync(request, queryParameters, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
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