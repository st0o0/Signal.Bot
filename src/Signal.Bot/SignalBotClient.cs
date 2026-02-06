using System;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Signal.Bot.Args;
using Signal.Bot.Exceptions;
using Signal.Bot.Internal;
using Signal.Bot.Requests;
using Signal.Bot.Serialization;
using R3;

namespace Signal.Bot;

public class SignalBotClient : ISignalBotClient
{
    private readonly HttpClient _httpClient;
    private readonly SignalBotClientOptions _options;
    private readonly Subject<OnApiRequestArgs> _onApiRequest = new();
    private readonly Subject<OnApiResponseArgs> _onApiResponse = new();
    private readonly Subject<Exception> _onException = new();

    public SignalBotClient(Action<SignalBotClientOptionsBuilder>? configure = null)
    {
        var builder = SignalBotClientOptionsBuilder.Create();
        configure?.Invoke(builder);
        _options = builder.Build();
        _httpClient = _options.HttpClient!;
        JsonSerializerOptions = JsonBotAPI.Options;
    }

    public string BaseUrl => _options.BaseUrl;
    public string Number => _options.Number;

    public JsonSerializerOptions JsonSerializerOptions { get; }

    public Observable<OnApiRequestArgs> OnApiRequest => _onApiRequest.AsObservable();
    public Observable<OnApiResponseArgs> OnApiResponse => _onApiResponse.AsObservable();
    public Observable<Exception> OnException => _onException.AsObservable();

    public async Task<HttpResponseMessage> SendAsync(IRequest request,
        IQueryParameterRegistry? queryParameters = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var methodName = request.MethodName;
            queryParameters ??= new QueryParameterRegistry();
            if (request is SearchNumbersRequest { Numbers: not null } searchRequest)
            {
                queryParameters.AddRange("numbers", searchRequest.Numbers);
            }

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
                    var error = await httpResponse.Content.ReadFromJsonAsync<Types.Error>(cancellationToken);
                    throw new HttpRequestException(error!.Message);
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

    public async Task SendRequestAsync(
        IRequest request,
        IQueryParameterRegistry? queryParameters = null,
        CancellationToken cancellationToken = default)
    {
        _ = await SendAsync(request, queryParameters, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async Task<TResponse> SendRequestAsync<TResponse>(IRequest<TResponse> request,
        IQueryParameterRegistry? queryParameters = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var httpResponse = await SendAsync(request, queryParameters, cancellationToken: cancellationToken);
            var content = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<TResponse>(content, JsonSerializerOptions)!;
            // return await httpResponse.Content
            //     .ReadFromJsonAsync<TResponse>(JsonBotAPI.Options, cancellationToken)
            //     .ConfigureAwait(false)!;
        }
        catch (Exception ex)
        {
            _onException.OnNext(ex);
            _onApiRequest.OnErrorResume(ex);
            _onApiResponse.OnErrorResume(ex);
            return default!;
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing) return;

        _httpClient.Dispose();
        _onApiRequest.OnCompleted();
        _onApiResponse.OnCompleted();
        _onException.OnCompleted();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}