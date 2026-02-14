using Signal.Bot.Types;

namespace Signal.Bot.Polling;

internal class DefaultReceivedMessageHandler(
    Func<ISignalBotClient, ReceivedMessageEnvelope, CancellationToken, Task> updateHandler,
    Func<ISignalBotClient, Error, CancellationToken, Task> errorHandler)
    : IReceivedMessageHandler
{
    public async Task HandleAsync(ISignalBotClient client, ReceivedMessageEnvelope messageEnvelope,
        CancellationToken cancellationToken)
        => await updateHandler(client, messageEnvelope, cancellationToken);

    public Task HandleErrorAsync(
        ISignalBotClient client,
        Error error,
        CancellationToken cancellationToken)
        => errorHandler(client, error, cancellationToken);
}