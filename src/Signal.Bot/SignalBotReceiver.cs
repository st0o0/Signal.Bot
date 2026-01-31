using System;
using System.Net.WebSockets;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IO;
using Signal.Bot.Polling;
using Signal.Bot.Types;
using Websocket.Client;

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
    private IWebsocketClient? _websocketClient;
    private IDisposable? _messageSubscription;
    private IDisposable? _errorSubscription;
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

        _websocketClient ??= new WebsocketClient(uri, memoryStreamManager: StreamManager.Value)
        {
            Name = "Signal.Bot",
            ConnectTimeout = TimeSpan.FromSeconds(30),
            ReconnectTimeout = TimeSpan.FromSeconds(10),
            ErrorReconnectTimeout = TimeSpan.FromSeconds(30),
            LostReconnectTimeout = TimeSpan.FromSeconds(30),
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

    private static string ConvertToWebSocketUrl(string baseUrl)
    {
        var cleanUrl = baseUrl
            .Replace("http://", "", StringComparison.OrdinalIgnoreCase)
            .Replace("https://", "", StringComparison.OrdinalIgnoreCase);

        var scheme = baseUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
            ? "wss"
            : "ws";

        return $"{scheme}://{cleanUrl}";
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
                .Stop(WebSocketCloseStatus.NormalClosure, "Disposed")
                .WaitAsync(TimeSpan.FromSeconds(1), CancellationToken.None) ?? Task.CompletedTask);
        }
        catch
        {
            /* Swallow on cleanup */
        }

        await (_disposeCts?.CancelAsync() ?? Task.CompletedTask);
        _linkedCts?.Dispose();
        _disposeCts?.Dispose();
        _websocketClient?.Dispose();
        _messageSubscription?.Dispose();
        _errorSubscription?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore();
    }
}