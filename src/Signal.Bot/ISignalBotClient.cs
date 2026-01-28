using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Signal.Bot.Args;
using Signal.Bot.Requests;

namespace Signal.Bot;

public interface ISignalBotClient
{
    string BaseUrl { get; }
    string Number { get; }
    JsonSerializerOptions JsonSerializerOptions { get; }
    CancellationToken GlobalCancelToken { get; }
    TimeSpan Timeout { get; }

    IObservable<OnApiRequestArgs> OnApiRequest { get; }
    IObservable<OnApiResponseArgs> OnApiResponse { get; }
    IObservable<Exception> OnException { get; }

    Task SendRequestAsync(
        IRequest request,
        string[]? queryParameters = null,
        CancellationToken cancellationToken = default);

    Task<TResponse> SendRequestAsync<TResponse>(
        IRequest<TResponse> request,
        string[]? queryParameters = null,
        CancellationToken cancellationToken = default
    );
}