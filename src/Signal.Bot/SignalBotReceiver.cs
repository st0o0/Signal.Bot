using System;
using System.Net.WebSockets;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Signal.Bot.Polling;
using Signal.Bot.Types;
using Websocket.Client;

namespace Signal.Bot;

internal sealed class SignalBotReceiver : IDisposable
{
    private readonly ISignalBotClient _client;
    private IWebsocketClient? _websocketClient;
    private IDisposable? _messageSubscription;
    private IDisposable? _errorSubscription;
    private CancellationTokenSource? _disposeCts;
    private CancellationTokenSource? _linkedCts;

    public SignalBotReceiver(ISignalBotClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    internal SignalBotReceiver(ISignalBotClient client, IWebsocketClient websocketClient)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _websocketClient = websocketClient ?? throw new ArgumentNullException(nameof(websocketClient));
    }

    public async Task<IDisposable> StartReceivingAsync(
        IReceivedMessageHandler handler,
        Action<ReceiverOptionsBuilder>? receiverOptionsConfigure = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(handler);

        _disposeCts = new CancellationTokenSource();
        _linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
            _client.GlobalCancelToken,
            cancellationToken,
            _disposeCts.Token);

        var builder = new ReceiverOptionsBuilder();
        receiverOptionsConfigure?.Invoke(builder);
        var options = builder.Build();

        var uri = new Uri($"ws://{_client.BaseUrl}/v1/receive/{_client.Number}" +
                          options.AsQueryParameter().Build());

        _websocketClient ??= new WebsocketClient(uri)
        {
            Name = "Signal.Bot",
            ConnectTimeout = TimeSpan.FromSeconds(30),
            ReconnectTimeout = TimeSpan.FromSeconds(10),
            ErrorReconnectTimeout = TimeSpan.FromSeconds(30),
            MessageEncoding = Encoding.UTF8
        };

        var messages = _websocketClient.MessageReceived
            .Select(msg => msg.MessageType switch
            {
                WebSocketMessageType.Binary when msg.Binary is not null => Encoding.UTF8.GetString(msg.Binary),
                WebSocketMessageType.Text when msg.Text is not null => msg.Text,
                _ => null
            })
            .Select(content => JsonSerializer.Deserialize<ReceivedMessage>(content, _client.JsonSerializerOptions))
            .Select(parsed => parsed!);

        var errors = Observable.Merge(
            messages
                .SelectMany(_ => Observable.Empty<Error>())
                .Catch((Exception ex) => Observable.Return(
                    new Error(ex, ErrorSource.MessageReceivedTermination))),
            _websocketClient.DisconnectionHappened
                .Select(info => info.To())
                .Catch((Exception ex) => Observable.Return(
                    new Error(ex, ErrorSource.DisconnectionHappenedTermination))),
            _websocketClient.ReconnectionHappened
                .Select(info => info.To())
                .Catch((Exception ex) => Observable.Return(
                    new Error(ex, ErrorSource.ReconnectionHappenedTermination)))
        );

        _messageSubscription = messages
            .ObserveOn(TaskPoolScheduler.Default)
            .Buffer(TimeSpan.FromMilliseconds(100), options.QueueCapacity)
            .SelectMany(x => x.ToObservable())
            .Select(message => Observable.FromAsync(async () =>
            {
                try
                {
                    await handler.HandleAsync(_client, message, _linkedCts.Token)
                        .ConfigureAwait(false);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    try
                    {
                        await handler.HandleErrorAsync(_client,
                                new Error(ex, ErrorSource.MessageReceived), _linkedCts.Token)
                            .ConfigureAwait(false);
                    }
                    catch
                    {
                        /* Swallow */
                    }
                }
            }))
            .Concat()
            .Subscribe(_ => { }, _ => { });

        _errorSubscription = errors
            .ObserveOn(TaskPoolScheduler.Default)
            .Select(error => Observable.FromAsync(async () =>
            {
                try
                {
                    await handler.HandleErrorAsync(_client, error, _linkedCts.Token)
                        .ConfigureAwait(false);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    try
                    {
                        await handler.HandleErrorAsync(_client,
                                new Error(ex, ErrorSource.FatalError), _linkedCts.Token)
                            .ConfigureAwait(false);
                    }
                    catch
                    {
                        /* Swallow */
                    }
                }
            }))
            .Concat()
            .Subscribe(_ => { }, _ => { });

        await _websocketClient.Start().ConfigureAwait(false);

        return this;
    }

    private void Dispose(bool disposing)
    {
        if (!disposing) return;
        _ = Task
            .Run(async () =>
            {
                try
                {
                    if (_websocketClient != null)
                    {
                        await _websocketClient
                            .Stop(WebSocketCloseStatus.NormalClosure, "Disposed")
                            .WaitAsync(TimeSpan.FromSeconds(3), CancellationToken.None);
                    }
                }
                catch
                {
                    /* Swallow on cleanup */
                }
            }, CancellationToken.None)
            .ContinueWith(_ =>
            {
                _disposeCts?.Cancel();
                _websocketClient?.Dispose();
                _linkedCts?.Dispose();
                _disposeCts?.Dispose();
                _messageSubscription?.Dispose();
                _errorSubscription?.Dispose();
            }, CancellationToken.None);
    }


    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~SignalBotReceiver()
    {
        Dispose(false);
    }
}