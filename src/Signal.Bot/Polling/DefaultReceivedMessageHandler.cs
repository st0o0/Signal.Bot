using Signal.Bot.Types;

namespace Signal.Bot.Polling;

public class DefaultReceivedMessageHandler(
    Func<ISignalBotClient, ReceivedMessage, CancellationToken, Task> updateHandler,
    Func<ISignalBotClient, Error, CancellationToken, Task> errorHandler)
    : IReceivedMessageHandler
{
    public async Task HandleAsync(ISignalBotClient client, ReceivedMessage message, CancellationToken cancellationToken)
        => await updateHandler(client, message, cancellationToken);

    public Task HandleErrorAsync(ISignalBotClient client, Error error, CancellationToken cancellationToken)
        => errorHandler(client, error, cancellationToken);
}