using System;
using System.Linq;
using System.Net.Http.Json;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Signal.Bot.Args;
using Signal.Bot.Exceptions;
using Signal.Bot.Requests;
using Signal.Bot.Serialization;

namespace Signal.Bot;

public class SignalBotClient : ISignalBotClient
{
    private readonly SignalBotClientOptions _options;
    private readonly HttpClient _httpClient;
    private readonly Subject<OnApiRequestArgs> _onApiRequest = new();
    private readonly Subject<OnApiResponseArgs> _onApiResponse = new();
    private readonly Subject<Exception> _onException = new();

    public SignalBotClient(string number, HttpClient httpClient, CancellationToken cancellationToken = default) : this(
        new SignalBotClientOptions(number,
            httpClient.BaseAddress!.GetComponents(UriComponents.HostAndPort, UriFormat.UriEscaped)), httpClient,
        cancellationToken)
    {
    }

    public SignalBotClient(SignalBotClientOptions options, HttpClient httpClient,
        CancellationToken cancellationToken = default)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _httpClient = httpClient;
        JsonSerializerOptions = options.JsonSerializerOptions;
        GlobalCancelToken = cancellationToken;
    }

    public string BaseUrl => _options.BaseUrl;

    public string Number => _options.Number;

    public JsonSerializerOptions JsonSerializerOptions { get; }

    public CancellationToken GlobalCancelToken { get; }

    public TimeSpan Timeout => _httpClient.Timeout;
    public IObservable<OnApiRequestArgs> OnApiRequest => _onApiRequest.AsObservable();
    public IObservable<OnApiResponseArgs> OnApiResponse => _onApiResponse.AsObservable();
    public IObservable<Exception> OnException => _onException.AsObservable();

    public async Task<TResponse> SendRequestAsync<TResponse>(IRequest<TResponse> request,
        string[]? queryParameters = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(GlobalCancelToken, cancellationToken);
            cancellationToken = cts.Token;
            var methodName = request.MethodName;
            var query = new List<string>(queryParameters ?? []);
            if (request is SearchNumbersRequest { Numbers: not null } searchRequest)
            {
                query.AddRange(searchRequest.Numbers.Select(num => $"numbers={Uri.EscapeDataString(num)}"));
            }

            if (query.Count > 0)
            {
                methodName += "?" + string.Join("&", query);
            }

            var httpRequest = new HttpRequestMessage(request.HttpMethod, methodName)
            {
                Content = request.ToHttpContent()
            };
            _onApiRequest.OnNext(new OnApiRequestArgs(request, httpRequest));
            HttpResponseMessage? httpResponse;
            try
            {
                httpResponse = await _httpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
                httpResponse.EnsureSuccessStatusCode();
            }
            catch (TaskCanceledException exception)
            {
                if (cancellationToken.IsCancellationRequested) throw;
                throw new RequestException("Bot API Request timed out", exception);
            }
            catch (Exception exception)
            {
                throw new RequestException($"Bot API Service Failure: {exception.GetType().Name}: {exception.Message}",
                    exception);
            }

            _onApiResponse.OnNext(new OnApiResponseArgs(request, httpRequest, httpResponse));
            try
            {
                return (await httpResponse.Content
                    .ReadFromJsonAsync<TResponse>(JsonBotAPI.Options, cancellationToken)
                    .ConfigureAwait(false))!;
            }
            catch (Exception exception)
            {
                throw new RequestException("There was an exception during deserialization of the response",
                    httpResponse.StatusCode, exception);
            }
        }
        catch (Exception ex)
        {
            _onException.OnNext(ex);
            _onApiRequest.OnError(ex);
            _onApiResponse.OnError(ex);
            return default!;
        }
    }

    public async Task SendRequestAsync(IRequest request, string[]? queryParameters = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var methodName = request.MethodName;
            var query = new List<string>(queryParameters ?? []);
            if (request is SearchNumbersRequest { Numbers: not null } searchRequest)
            {
                query.AddRange(searchRequest.Numbers.Select(num => $"numbers={Uri.EscapeDataString(num)}"));
            }

            if (query.Count > 0)
            {
                methodName += "?" + string.Join("&", query);
            }

            var httpRequest = new HttpRequestMessage(request.HttpMethod, methodName)
            {
                Content = request.ToHttpContent()
            };

            _onApiRequest.OnNext(new OnApiRequestArgs(request, httpRequest));
            using var httpResponse = await _httpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            _onApiResponse.OnNext(new OnApiResponseArgs(request, httpRequest, httpResponse));
            httpResponse.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _onException.OnNext(ex);
            _onApiRequest.OnError(ex);
            _onApiResponse.OnError(ex);
        }
    }
}