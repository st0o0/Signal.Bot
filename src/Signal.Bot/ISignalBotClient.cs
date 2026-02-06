using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using R3;
using Signal.Bot.Args;
using Signal.Bot.Internal;
using Signal.Bot.Requests;

namespace Signal.Bot;

public interface ISignalBotClient : IDisposable
{
    string BaseUrl { get; }
    string Number { get; }
    JsonSerializerOptions JsonSerializerOptions { get; }

    Observable<OnApiRequestArgs> OnApiRequest { get; }
    Observable<OnApiResponseArgs> OnApiResponse { get; }
    Observable<Exception> OnException { get; }

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