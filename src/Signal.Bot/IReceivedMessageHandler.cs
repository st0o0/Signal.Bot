using Signal.Bot.Types;

namespace Signal.Bot;

/// <summary>
/// Defines the contract for handling incoming Signal messages and errors during polling operations.
/// </summary>
public interface IReceivedMessageHandler
{
    /// <summary>
    /// Handles an incoming Signal message received from the polling mechanism.
    /// </summary>
    /// <param name="client">The <see cref="ISignalBotClient"/> instance that received the message.</param>
    /// <param name="messageEnvelope">The <see cref="ReceivedMessageEnvelope"/> containing the message data and metadata.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task HandleAsync(
        ISignalBotClient client,
        ReceivedMessageEnvelope messageEnvelope,
        CancellationToken cancellationToken);

    /// <summary>
    /// Handles errors that occur during message polling or processing.
    /// </summary>
    /// <param name="client">The <see cref="ISignalBotClient"/> instance where the error occurred.</param>
    /// <param name="error">The <see cref="Error"/> containing details about the error that occurred.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous error handling operation.</returns>
    Task HandleErrorAsync(
        ISignalBotClient client,
        Error error,
        CancellationToken cancellationToken);
}