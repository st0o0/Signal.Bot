using System;
using System.Threading;
using System.Threading.Tasks;
using Signal.Bot.Types;

namespace Signal.Bot.Polling;

public class DefaultReceivedMessageHandler(
    Func<ISignalBotClient, ReceivedMessage, CancellationToken, Task> updateHandler,
    Func<ISignalBotClient, IError, CancellationToken, Task> errorHandler)
    : IReceivedMessageHandler
{
    public async Task HandleAsync(ISignalBotClient client, ReceivedMessage message,
        CancellationToken cancellationToken)
        => await updateHandler(client, message, cancellationToken);

    public Task HandleErrorAsync(
        ISignalBotClient client,
        IError error,
        CancellationToken cancellationToken)
        => errorHandler(client, error, cancellationToken);
}