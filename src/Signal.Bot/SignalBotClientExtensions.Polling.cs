using System;
using System.Net.WebSockets;
using System.Reactive.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Signal.Bot.Internal;
using Signal.Bot.Polling;
using Signal.Bot.Types;
using Websocket.Client;

namespace Signal.Bot;

public static partial class SignalBotClientExtensions
{
    public static void StartReceiving<TUpdateHandler>(this ISignalBotClient botClient,
        ReceiverOptions? receiverOptions = null,
        CancellationToken cancellationToken = default) where TUpdateHandler : IReceivedMessageHandler, new()
        => botClient.StartReceiving(new TUpdateHandler(), receiverOptions, cancellationToken);

    public static void StartReceiving(this ISignalBotClient botClient,
        Func<ISignalBotClient, ReceivedMessage, CancellationToken, Task> updateHandler,
        Func<ISignalBotClient, IError, CancellationToken, Task> errorHandler,
        ReceiverOptions? receiverOptions = null, CancellationToken cancellationToken = default)
        => botClient.StartReceiving(new DefaultReceivedMessageHandler(updateHandler, errorHandler), receiverOptions,
            cancellationToken);

    public static void StartReceiving(this ISignalBotClient botClient,
        Action<ISignalBotClient, ReceivedMessage, CancellationToken> updateHandler,
        Action<ISignalBotClient, IError, CancellationToken> errorHandler,
        ReceiverOptions? receiverOptions = null, CancellationToken cancellationToken = default)
        => botClient.StartReceiving(new DefaultReceivedMessageHandler(
            (bot, update, token) =>
            {
                updateHandler(bot, update, token);
                return Task.CompletedTask;
            },
            (bot, err, token) =>
            {
                errorHandler(bot, err, token);
                return Task.CompletedTask;
            }
        ), receiverOptions, cancellationToken);

    public static void StartReceiving(this ISignalBotClient client, IReceivedMessageHandler handler,
        ReceiverOptions? receiverOptions = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(handler);

        // ReSharper disable once MethodSupportsCancellation
        _ = Task.Run(async () =>
        {
            try
            {
                await client
                    .ReceiveAsync(handler, (receiverOptions ?? new ReceiverOptions()).AsQueryParameter(),
                        receiverOptions?.QueueCapacity, cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // normal shutdown
            }
            catch (Exception ex)
            {
                await handler.HandleErrorAsync(
                        client,
                        new Error(ex, ErrorSource.FatalError),
                        cancellationToken)
                    .ConfigureAwait(false);
            }
        }, cancellationToken);
    }

    public static async Task ReceiveAsync<TUpdateHandler>(this ISignalBotClient client,
        ReceiverOptions? receiverOptions = null,
        CancellationToken cancellationToken = default) where TUpdateHandler : IReceivedMessageHandler, new()
        => await client.ReceiveAsync(
            new TUpdateHandler(),
            (receiverOptions ?? new ReceiverOptions()).AsQueryParameter(),
            receiverOptions?.QueueCapacity,
            cancellationToken).ConfigureAwait(false);

    public static async Task ReceiveAsync(this ISignalBotClient client,
        Func<ISignalBotClient, ReceivedMessage, CancellationToken, Task> updateHandler,
        Func<ISignalBotClient, IError, CancellationToken, Task> errorHandler,
        ReceiverOptions? receiverOptions = null, CancellationToken cancellationToken = default)
        => await client.ReceiveAsync(
            new DefaultReceivedMessageHandler(updateHandler, errorHandler),
            (receiverOptions ?? new ReceiverOptions()).AsQueryParameter(),
            receiverOptions?.QueueCapacity,
            cancellationToken).ConfigureAwait(false);

    public static async Task ReceiveAsync(this ISignalBotClient botClient,
        Action<ISignalBotClient, ReceivedMessage, CancellationToken> updateHandler,
        Action<ISignalBotClient, IError, CancellationToken> errorHandler,
        ReceiverOptions? receiverOptions = null, CancellationToken cancellationToken = default)
        => await botClient.ReceiveAsync(new DefaultReceivedMessageHandler(
                (bot, update, token) =>
                {
                    updateHandler(bot, update, token);
                    return Task.CompletedTask;
                },
                (bot, err, token) =>
                {
                    errorHandler(bot, err, token);
                    return Task.CompletedTask;
                }
            ), (receiverOptions ?? new ReceiverOptions()).AsQueryParameter(), receiverOptions?.QueueCapacity,
            cancellationToken).ConfigureAwait(false);

    public static async Task<IDisposable> ReceiveAsync(this ISignalBotClient client,
        IReceivedMessageHandler handler,
        IQueryParameterRegistry queryParameter,
        int? queueCapacity = null,
        CancellationToken cancellationToken = default)
    {
        queueCapacity ??= 100;
        return await client.InternalReceiveAsync(handler, queryParameter, queueCapacity.Value, cancellationToken)
            .ConfigureAwait(false);
    }

    private static async Task<IDisposable> InternalReceiveAsync(this ISignalBotClient client,
        IReceivedMessageHandler handler,
        IQueryParameterRegistry queryParameter,
        int queueCapacity,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(handler);
        using var cts =
            CancellationTokenSource.CreateLinkedTokenSource(client.GlobalCancelToken, cancellationToken);
        cancellationToken = cts.Token;

        var messageChannel = Channel
            .CreateBounded<ReceivedMessage>(new BoundedChannelOptions(queueCapacity)
            {
                SingleReader = true,
                SingleWriter = true,
                FullMode = BoundedChannelFullMode.Wait
            });

        var errorChannel = Channel
            .CreateBounded<Error>(new BoundedChannelOptions(queueCapacity)
            {
                SingleReader = true,
                SingleWriter = true,
                FullMode = BoundedChannelFullMode.Wait
            });
        var errorWriter = errorChannel.Writer;
        var errorReader = errorChannel.Reader;
        var messageWriter = messageChannel.Writer;
        var messageReader = messageChannel.Reader;

        var uri = new Uri($"ws://{client.BaseUrl}/v1/receive/{client.Number}" + queryParameter.Build());

        var wsClient = new WebsocketClient(uri);
        wsClient.ConnectTimeout = TimeSpan.FromSeconds(30);
        wsClient.ReconnectTimeout = TimeSpan.FromSeconds(10);
        wsClient.ErrorReconnectTimeout = TimeSpan.FromSeconds(30);
        wsClient.MessageEncoding = Encoding.UTF8;

        wsClient.MessageReceived
            .Select(x => x.MessageType switch
            {
                WebSocketMessageType.Binary => Encoding.UTF8.GetString(x.Binary!),
                WebSocketMessageType.Text => x.Text!,
                _ => null
            })
            .Select(content => JsonSerializer.Deserialize<ReceivedMessage>(content!, client.JsonSerializerOptions))
            .Where(x => x is not null)
            .Subscribe(
                msg => messageWriter.TryWrite(msg!),
                ex => errorWriter.TryWrite(new Error(ex, ErrorSource.MessageReceivedTermination)));

        wsClient.DisconnectionHappened
            .Subscribe(
                info => errorWriter.TryWrite(new DisconnectionInfoError(info)),
                ex => errorWriter.TryWrite(new Error(ex, ErrorSource.DisconnectionHappenedTermination)));

        wsClient.ReconnectionHappened
            .Subscribe(
                info => errorWriter.TryWrite(new ReconnectionInfoError(info)),
                ex => errorWriter.TryWrite(new Error(ex, ErrorSource.ReconnectionHappenedTermination)));

        await wsClient.Start();
        var messageHandlerTask = Parallel
            .ForEachAsync(
                messageReader.ReadAllAsync(cancellationToken), new ParallelOptions
                {
                    CancellationToken = cancellationToken,
                    MaxDegreeOfParallelism = 1
                },
                async (message, token) =>
                {
                    try
                    {
                        await handler
                            .HandleAsync(client, message, token)
                            .ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        await handler
                            .HandleErrorAsync(client, new Error(ex, ErrorSource.MessageReceived), token)
                            .ConfigureAwait(false);
                    }
                });

        var errorHandlerTask = Parallel
            .ForEachAsync(
                errorReader.ReadAllAsync(cancellationToken), new ParallelOptions
                {
                    CancellationToken = cancellationToken,
                    MaxDegreeOfParallelism = 1
                },
                async (error, token) =>
                {
                    try
                    {
                        await handler
                            .HandleErrorAsync(client, error, token)
                            .ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        await handler
                            .HandleErrorAsync(client, new Error(ex, ErrorSource.FatalError), token)
                            .ConfigureAwait(false);
                    }
                });


        await Task.WhenAll(messageHandlerTask, errorHandlerTask).ConfigureAwait(false);
        return wsClient;
    }

    public static IQueryParameterRegistry AsQueryParameter(this ReceiverOptions options)
    {
        var result = new QueryParameterRegistry();
        result.Add("timeout", options.Timeout, x => x.Seconds.ToString());
        result.Add("ignore_attachments", options.IgnoreAttachments);
        result.Add("ignore_stories", options.IgnoreStories);
        result.Add("max_messages", options.MaxMessage);
        result.Add("send_read_receipts", options.SendReadReceipts);

        return result;
    }
}