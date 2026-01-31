using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Signal.Bot.Args;
using Signal.Bot.Internal;
using Signal.Bot.Requests;

namespace Signal.Bot;

public interface ISignalBotClient : IDisposable
{
    string BaseUrl { get; }
    string Number { get; }
    JsonSerializerOptions JsonSerializerOptions { get; }

    IObservable<OnApiRequestArgs> OnApiRequest { get; }
    IObservable<OnApiResponseArgs> OnApiResponse { get; }
    IObservable<Exception> OnException { get; }

    Task<HttpResponseMessage> SendAsync(
        IRequest request,
        IQueryParameterRegistry? queryParameters = null,
        CancellationToken cancellationToken = default);

    Task SendRequestAsync(
        IRequest request,
        IQueryParameterRegistry? queryParameters = null,
        CancellationToken cancellationToken = default);

    Task<TResponse> SendRequestAsync<TResponse>(
        IRequest<TResponse> request,
        IQueryParameterRegistry? queryParameters = null,
        CancellationToken cancellationToken = default
    );
}