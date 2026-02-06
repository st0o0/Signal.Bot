using System;
using System.Net.WebSockets;
using R3;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IO;
using Signal.Bot.Polling;
using WebSocket.Rx;
using ReceivedMessage = Signal.Bot.Types.ReceivedMessage;

namespace Signal.Bot;

internal sealed class SignalBotReceiver : IAsyncDisposable
{
    private static readonly Lazy<RecyclableMemoryStreamManager> StreamManager = new(() =>
        new RecyclableMemoryStreamManager(new RecyclableMemoryStreamManager.Options
        {
            BlockSize = 1024, // 1KB blocks
            LargeBufferMultiple = 1024 * 1024, // 1MB for large buffers
            MaximumBufferSize = 16 * 1024 * 1024, // 16MB max
            GenerateCallStacks = false, // Disable in production
            AggressiveBufferReturn = true, // Return buffers quickly
            MaximumLargePoolFreeBytes = 16 * 1024 * 1024 * 4, // 64MB pool max
            MaximumSmallPoolFreeBytes = 100 * 1024 * 1024 // 100MB small pool
        }));

    private int _disposed;
    private readonly ISignalBotClient _client;
    private ReactiveWebSocketClient? _websocketClient;
    private CompositeDisposable? _disposables;
    private CancellationTokenSource? _disposeCts;
    private CancellationTokenSource? _linkedCts;


    public SignalBotReceiver(ISignalBotClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<IAsyncDisposable> StartReceivingAsync(
        IReceivedMessageHandler handler,
        Action<ReceiverOptionsBuilder>? receiverOptionsConfigure = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(handler);

        _disposeCts = new CancellationTokenSource();
        _linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken,
            _disposeCts.Token);

        var builder = new ReceiverOptionsBuilder();
        receiverOptionsConfigure?.Invoke(builder);
        var options = builder.Build();

        var uri = new Uri($"{ConvertToWebSocketUrl(_client.BaseUrl)}/v1/receive/{_client.Number}" +
                          options.AsQueryParameter().Build());

        _websocketClient ??= new ReactiveWebSocketClient(uri, memoryStreamManager: StreamManager.Value)
        {
            ConnectTimeout = TimeSpan.FromSeconds(30),
            KeepAliveInterval = TimeSpan.FromSeconds(30),
            KeepAliveTimeout = TimeSpan.FromSeconds(10),
            IsReconnectionEnabled = true,
            IsTextMessageConversionEnabled = true,
            MessageEncoding = Encoding.UTF8
        };

        var messages = _websocketClient.MessageReceived
            .Select(msg => msg.MessageType switch
            {
                WebSocketMessageType.Binary when msg.Binary is not null => Encoding.UTF8.GetString(msg.Binary),
                WebSocketMessageType.Text when msg.Text is not null => msg.Text,
                _ => null
            })
            .Select(content => JsonSerializer.Deserialize<ReceivedMessage>(content!, _client.JsonSerializerOptions))
            .Where(msg => msg?.Envelope?.ReceiptMessage is null || !options.IgnoreReceipt)
            .Where(msg => msg?.Envelope?.TypingMessage is null || !options.IgnoreTyping)
            .Where(msg => msg?.Envelope?.SyncMessage is null || !options.IgnoreSync)
            .Select(parsed => parsed!);

        var messageSubscription = messages
            .SubscribeAwait(async (msg, ct) =>
            {
                try
                {
                    using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_linkedCts.Token, ct);
                    await handler.HandleAsync(_client, msg, linkedCts.Token).ConfigureAwait(false);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    try
                    {
                        await handler
                            .HandleErrorAsync(_client, new Error(ex, ErrorType.MessageReceived), _linkedCts.Token)
                            .ConfigureAwait(false);
                    }
                    catch
                    {
                        // noop
                    }
                }
            }, _ => { });

        var errors = Observable.Merge(
            messages
                .SelectMany(_ => Observable.Empty<Error>())
                .Catch((Exception ex) => Observable.Return(new Error(ex, ErrorType.MessageReceivedTermination))),
            _websocketClient.DisconnectionHappened
                .Select(info => info.To())
                .Catch((Exception ex) =>
                    Observable.Return(new Error(ex, ErrorType.DisconnectionHappenedTermination))),
            _websocketClient.ConnectionHappened
                .Select(info => info.To())
                .Catch((Exception ex) => Observable.Return(new Error(ex, ErrorType.ConnectionHappenedTermination)))
        );

        var errorSubscription = errors
            .SubscribeAwait(async (error, ct) =>
            {
                try
                {
                    using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_linkedCts.Token, ct);
                    await handler.HandleErrorAsync(_client, error, linkedCts.Token)
                        .ConfigureAwait(false);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    try
                    {
                        await handler.HandleErrorAsync(_client,
                                new Error(ex, ErrorType.FatalError), _linkedCts.Token)
                            .ConfigureAwait(false);
                    }
                    catch
                    {
                        // noop
                    }
                }
            }, result => result.TryThrow());

        _disposables = new CompositeDisposable
        {
            errorSubscription,
            messageSubscription
        };

        await _websocketClient.StartAsync().ConfigureAwait(false);

        return this;
    }

    private static string ConvertToWebSocketUrl(string baseUrl)
    {
        var tempUrl = baseUrl
            .Replace("http://", "ws://", StringComparison.OrdinalIgnoreCase)
            .Replace("https://", "wss://", StringComparison.OrdinalIgnoreCase);
        return tempUrl.StartsWith("ws") ? tempUrl : $"ws://{tempUrl}";
    }

    private async ValueTask DisposeAsyncCore()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return;
        }

        try
        {
            await (_websocketClient?
                .StopAsync(WebSocketCloseStatus.NormalClosure, "Disposed")
                .WaitAsync(TimeSpan.FromSeconds(1), CancellationToken.None) ?? Task.CompletedTask);
        }
        catch
        {
            // noop
        }

        await (_disposeCts?.CancelAsync() ?? Task.CompletedTask);
        _linkedCts?.Dispose();
        _disposeCts?.Dispose();
        await (_websocketClient?.DisposeAsync() ?? ValueTask.CompletedTask);
        _disposables?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore();
    }
}