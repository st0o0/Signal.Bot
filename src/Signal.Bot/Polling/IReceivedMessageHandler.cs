using System.Threading;
using System.Threading.Tasks;
using Signal.Bot.Types;

namespace Signal.Bot.Polling;

public interface IReceivedMessageHandler
{
    Task HandleAsync(
        ISignalBotClient client,
        ReceivedMessage message,
        CancellationToken cancellationToken);

    Task HandleErrorAsync(
        ISignalBotClient client,
        IError error,
        CancellationToken cancellationToken);
}