using System;
using System.Threading;
using System.Threading.Tasks;
using Signal.Bot.Internal;
using Signal.Bot.Polling;
using Signal.Bot.Types;

namespace Signal.Bot;

public static partial class SignalBotClientExtensions
{
    public static void StartReceiving<TUpdateHandler>(this ISignalBotClient botClient,
        Action<ReceiverOptionsBuilder>? receiverOptionsConfigure = null,
        CancellationToken cancellationToken = default) where TUpdateHandler : IReceivedMessageHandler, new()
        => botClient.StartReceiving(new TUpdateHandler(), receiverOptionsConfigure, cancellationToken);

    public static void StartReceiving(this ISignalBotClient botClient,
        Func<ISignalBotClient, ReceivedMessage, CancellationToken, Task> updateHandler,
        Func<ISignalBotClient, Error, CancellationToken, Task> errorHandler,
        Action<ReceiverOptionsBuilder>? receiverOptionsConfigure = null, CancellationToken cancellationToken = default)
        => botClient.StartReceiving(new DefaultReceivedMessageHandler(updateHandler, errorHandler),
            receiverOptionsConfigure,
            cancellationToken);

    public static void StartReceiving(this ISignalBotClient botClient,
        Action<ISignalBotClient, ReceivedMessage, CancellationToken> updateHandler,
        Action<ISignalBotClient, Error, CancellationToken> errorHandler,
        Action<ReceiverOptionsBuilder>? receiverOptionsConfigure = null, CancellationToken cancellationToken = default)
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
        ), receiverOptionsConfigure, cancellationToken);

    public static void StartReceiving(this ISignalBotClient client, IReceivedMessageHandler handler,
        Action<ReceiverOptionsBuilder>? receiverOptionsConfigure = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(handler);

        _ = Task.Run(async () =>
        {
            IAsyncDisposable? disposable = null;
            try
            {
                disposable = await client
                    .ReceiveAsync(handler, receiverOptionsConfigure, cancellationToken)
                    .ConfigureAwait(false);

                await Task
                    .Delay(Timeout.Infinite, cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await handler.HandleErrorAsync(
                        client,
                        new Error(ex, ErrorType.FatalError),
                        cancellationToken)
                    .ConfigureAwait(false);
            }
            finally
            {
                if (disposable is not null)
                {
                    await disposable.DisposeAsync().ConfigureAwait(false);
                }
            }
        }, CancellationToken.None);
    }

    public static async Task<IAsyncDisposable> ReceiveAsync<TUpdateHandler>(this ISignalBotClient client,
        Action<ReceiverOptionsBuilder>? receiverOptionsConfigure = null,
        CancellationToken cancellationToken = default) where TUpdateHandler : IReceivedMessageHandler, new()
        => await client.ReceiveAsync(
            new TUpdateHandler(),
            receiverOptionsConfigure,
            cancellationToken).ConfigureAwait(false);

    public static async Task<IAsyncDisposable> ReceiveAsync(this ISignalBotClient client,
        Func<ISignalBotClient, ReceivedMessage, CancellationToken, Task> updateHandler,
        Func<ISignalBotClient, Error, CancellationToken, Task> errorHandler,
        Action<ReceiverOptionsBuilder>? receiverOptionsConfigure = null,
        CancellationToken cancellationToken = default)
        => await client.ReceiveAsync(
            new DefaultReceivedMessageHandler(updateHandler, errorHandler),
            receiverOptionsConfigure,
            cancellationToken).ConfigureAwait(false);

    public static async Task<IAsyncDisposable> ReceiveAsync(this ISignalBotClient botClient,
        Action<ISignalBotClient, ReceivedMessage, CancellationToken> updateHandler,
        Action<ISignalBotClient, Error, CancellationToken> errorHandler,
        Action<ReceiverOptionsBuilder>? receiverOptionsConfigure = null,
        CancellationToken cancellationToken = default)
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
            ), receiverOptionsConfigure,
            cancellationToken).ConfigureAwait(false);

    public static async Task<IAsyncDisposable> ReceiveAsync(this ISignalBotClient client,
        IReceivedMessageHandler handler,
        Action<ReceiverOptionsBuilder>? receiverOptionsConfigure = null,
        CancellationToken cancellationToken = default)
        => await client.InternalReceiveAsync(handler, receiverOptionsConfigure, cancellationToken)
            .ConfigureAwait(false);

    private static async Task<IAsyncDisposable> InternalReceiveAsync(
        this ISignalBotClient client,
        IReceivedMessageHandler handler,
        Action<ReceiverOptionsBuilder>? receiverOptionsConfigure = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(handler);

        var receiver = new SignalBotReceiver(client);
        return await receiver
            .StartReceivingAsync(handler, receiverOptionsConfigure, cancellationToken)
            .ConfigureAwait(false);
    }

    internal static IQueryParameterRegistry AsQueryParameter(this ReceiverOptions options)
    {
        var result = new QueryParameterRegistry();
        result.Add("timeout", options.Timeout, x => x.Seconds.ToString());
        result.Add("ignore_attachments", options.IgnoreAttachments);
        result.Add("ignore_stories", options.IgnoreStories);
        result.Add("max_messages", options.MaxMessages);
        result.Add("send_read_receipts", options.SendReadReceipts);

        return result;
    }
}