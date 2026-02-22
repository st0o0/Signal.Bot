using Signal.Bot.Polling;
using Signal.Bot.Types;

namespace Signal.Bot;

/// <summary>
/// Provides extension methods for <see cref="ISignalBotClient"/> to enable message polling and receiving functionality.
/// These methods allow starting message receivers in various modes including fire-and-forget background tasks 
/// and controlled lifecycle management with disposable handles.
/// </summary>
public static class PollingExtensions
{
    /// <summary>
    /// Starts receiving messages using a strongly-typed handler class in a fire-and-forget manner.
    /// </summary>
    /// <typeparam name="TUpdateHandler">The type of handler implementing <see cref="IReceivedMessageHandler"/>.</typeparam>
    /// <param name="client">The Signal bot client instance.</param>
    /// <param name="receiverOptionsConfigure">Optional action to configure receiver options via <see cref="ReceiverOptionsBuilder"/>.</param>
    /// <param name="cancellationToken">Cancellation token to stop receiving messages.</param>
    public static void StartReceiving<TUpdateHandler>(this ISignalBotClient client,
        Action<ReceiverOptionsBuilder>? receiverOptionsConfigure = null,
        CancellationToken cancellationToken = default) where TUpdateHandler : IReceivedMessageHandler, new()
        => client.StartReceiving(new TUpdateHandler(), receiverOptionsConfigure, cancellationToken);

    /// <summary>
    /// Starts receiving messages using async delegate handlers for updates and errors in a fire-and-forget manner.
    /// </summary>
    /// <param name="client">The Signal bot client instance.</param>
    /// <param name="updateHandler">Async delegate to handle received messages.</param>
    /// <param name="errorHandler">Async delegate to handle errors that occur during message reception.</param>
    /// <param name="receiverOptionsConfigure">Optional action to configure receiver options via <see cref="ReceiverOptionsBuilder"/>.</param>
    /// <param name="cancellationToken">Cancellation token to stop receiving messages.</param>
    public static void StartReceiving(this ISignalBotClient client,
        Func<ISignalBotClient, ReceivedMessage, CancellationToken, Task> updateHandler,
        Func<ISignalBotClient, Error, CancellationToken, Task> errorHandler,
        Action<ReceiverOptionsBuilder>? receiverOptionsConfigure = null,
        CancellationToken cancellationToken = default)
        => client.StartReceiving(new DefaultReceivedMessageHandler(updateHandler, errorHandler),
            receiverOptionsConfigure,
            cancellationToken);

    /// <summary>
    /// Starts receiving messages using synchronous delegate handlers for updates and errors in a fire-and-forget manner.
    /// </summary>
    /// <param name="client">The Signal bot client instance.</param>
    /// <param name="updateHandler">Synchronous delegate to handle received messages.</param>
    /// <param name="errorHandler">Synchronous delegate to handle errors that occur during message reception.</param>
    /// <param name="receiverOptionsConfigure">Optional action to configure receiver options via <see cref="ReceiverOptionsBuilder"/>.</param>
    /// <param name="cancellationToken">Cancellation token to stop receiving messages.</param>
    public static void StartReceiving(this ISignalBotClient client,
        Action<ISignalBotClient, ReceivedMessage, CancellationToken> updateHandler,
        Action<ISignalBotClient, Error, CancellationToken> errorHandler,
        Action<ReceiverOptionsBuilder>? receiverOptionsConfigure = null,
        CancellationToken cancellationToken = default)
        => client.StartReceiving(new DefaultReceivedMessageHandler(
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

    /// <summary>
    /// Starts receiving messages using a custom handler implementation in a fire-and-forget manner.
    /// The receiver runs on a background task that continues until cancelled or an error occurs.
    /// </summary>
    /// <param name="client">The Signal bot client instance.</param>
    /// <param name="handler">The handler implementing <see cref="IReceivedMessageHandler"/> to process messages and errors.</param>
    /// <param name="receiverOptionsConfigure">Optional action to configure receiver options via <see cref="ReceiverOptionsBuilder"/>.</param>
    /// <param name="cancellationToken">Cancellation token to stop receiving messages.</param>
    /// <exception cref="ArgumentNullException">Thrown when botClient or handler is null.</exception>
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
                        new Error(ex, FailureSource.Failed),
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

    /// <summary>
    /// Starts receiving messages using a strongly-typed handler class and returns a disposable to control the receiver lifecycle.
    /// </summary>
    /// <typeparam name="TUpdateHandler">The type of handler implementing <see cref="IReceivedMessageHandler"/>.</typeparam>
    /// <param name="client">The Signal bot client instance.</param>
    /// <param name="receiverOptionsConfigure">Optional action to configure receiver options via <see cref="ReceiverOptionsBuilder"/>.</param>
    /// <param name="cancellationToken">Cancellation token to stop receiving messages.</param>
    /// <returns>An <see cref="IAsyncDisposable"/> that can be disposed to stop receiving messages.</returns>
    public static async Task<IAsyncDisposable> ReceiveAsync<TUpdateHandler>(this ISignalBotClient client,
        Action<ReceiverOptionsBuilder>? receiverOptionsConfigure = null,
        CancellationToken cancellationToken = default) where TUpdateHandler : IReceivedMessageHandler, new()
        => await client.ReceiveAsync(
            new TUpdateHandler(),
            receiverOptionsConfigure,
            cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Starts receiving messages using async delegate handlers and returns a disposable to control the receiver lifecycle.
    /// </summary>
    /// <param name="client">The Signal bot client instance.</param>
    /// <param name="updateHandler">Async delegate to handle received messages.</param>
    /// <param name="errorHandler">Async delegate to handle errors that occur during message reception.</param>
    /// <param name="receiverOptionsConfigure">Optional action to configure receiver options via <see cref="ReceiverOptionsBuilder"/>.</param>
    /// <param name="cancellationToken">Cancellation token to stop receiving messages.</param>
    /// <returns>An <see cref="IAsyncDisposable"/> that can be disposed to stop receiving messages.</returns>
    public static async Task<IAsyncDisposable> ReceiveAsync(this ISignalBotClient client,
        Func<ISignalBotClient, ReceivedMessage, CancellationToken, Task> updateHandler,
        Func<ISignalBotClient, Error, CancellationToken, Task> errorHandler,
        Action<ReceiverOptionsBuilder>? receiverOptionsConfigure = null,
        CancellationToken cancellationToken = default)
        => await client.ReceiveAsync(
            new DefaultReceivedMessageHandler(updateHandler, errorHandler),
            receiverOptionsConfigure,
            cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Starts receiving messages using synchronous delegate handlers and returns a disposable to control the receiver lifecycle.
    /// </summary>
    /// <param name="client">The Signal bot client instance.</param>
    /// <param name="updateHandler">Synchronous delegate to handle received messages.</param>
    /// <param name="errorHandler">Synchronous delegate to handle errors that occur during message reception.</param>
    /// <param name="receiverOptionsConfigure">Optional action to configure receiver options via <see cref="ReceiverOptionsBuilder"/>.</param>
    /// <param name="cancellationToken">Cancellation token to stop receiving messages.</param>
    /// <returns>An <see cref="IAsyncDisposable"/> that can be disposed to stop receiving messages.</returns>
    public static async Task<IAsyncDisposable> ReceiveAsync(this ISignalBotClient client,
        Action<ISignalBotClient, ReceivedMessage, CancellationToken> updateHandler,
        Action<ISignalBotClient, Error, CancellationToken> errorHandler,
        Action<ReceiverOptionsBuilder>? receiverOptionsConfigure = null,
        CancellationToken cancellationToken = default)
        => await client.ReceiveAsync(new DefaultReceivedMessageHandler(
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

    /// <summary>
    /// Starts receiving messages using a custom handler implementation and returns a disposable to control the receiver lifecycle.
    /// </summary>
    /// <param name="client">The Signal bot client instance.</param>
    /// <param name="handler">The handler implementing <see cref="IReceivedMessageHandler"/> to process messages and errors.</param>
    /// <param name="receiverOptionsConfigure">Optional action to configure receiver options via <see cref="ReceiverOptionsBuilder"/>.</param>
    /// <param name="cancellationToken">Cancellation token to stop receiving messages.</param>
    /// <returns>An <see cref="IAsyncDisposable"/> that can be disposed to stop receiving messages.</returns>
    public static async Task<IAsyncDisposable> ReceiveAsync(this ISignalBotClient client,
        IReceivedMessageHandler handler,
        Action<ReceiverOptionsBuilder>? receiverOptionsConfigure = null,
        CancellationToken cancellationToken = default)
        => await client.InternalReceiveAsync(handler, receiverOptionsConfigure, cancellationToken)
            .ConfigureAwait(false);

    private static async Task<IAsyncDisposable> InternalReceiveAsync(this ISignalBotClient client,
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


}