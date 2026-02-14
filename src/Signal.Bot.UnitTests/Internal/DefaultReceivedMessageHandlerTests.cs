using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Signal.Bot.Polling;
using Signal.Bot.Types;

namespace Signal.Bot.UnitTests.Internal;

public class DefaultReceivedMessageHandlerTests
{
    private readonly Func<ISignalBotClient, ReceivedMessageEnvelope, CancellationToken, Task> _updateHandlerMock;
    private readonly Func<ISignalBotClient, Error, CancellationToken, Task> _errorHandlerMock;
    private readonly DefaultReceivedMessageHandler _handler;
    private readonly ISignalBotClient _clientMock;
    private readonly ReceivedMessageEnvelope _messageEnvelope;
    private readonly Error _error;
    private readonly CancellationToken _cancellationToken;

    public DefaultReceivedMessageHandlerTests()
    {
        _updateHandlerMock = Substitute.For<Func<ISignalBotClient, ReceivedMessageEnvelope, CancellationToken, Task>>();
        _errorHandlerMock = Substitute.For<Func<ISignalBotClient, Error, CancellationToken, Task>>();

        _handler = new DefaultReceivedMessageHandler(_updateHandlerMock, _errorHandlerMock);

        _clientMock = Substitute.For<ISignalBotClient>();
        _messageEnvelope = new ReceivedMessageEnvelope { Envelope = new Envelope { SourceNumber = "test", SourceId = Guid.NewGuid() } };
        _error = new Error(null, FailureSource.Failed);
        _cancellationToken = CancellationToken.None;
    }

    [Fact(Timeout = 5000)]
    public async Task HandleAsync_CallsUpdateHandlerWithCorrectParameters()
    {
        // Act
        await _handler.HandleAsync(_clientMock, _messageEnvelope, _cancellationToken);

        // Assert
        await _updateHandlerMock.Received(1)(Arg.Is(_clientMock), Arg.Is(_messageEnvelope), Arg.Is(_cancellationToken));
    }


    [Fact(Timeout = 5000)]
    public async Task HandleAsync_UpdateHandlerThrowsException_PropagatesException()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        _updateHandlerMock(Arg.Any<ISignalBotClient>(), Arg.Any<ReceivedMessageEnvelope>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(exception);

        // Act & Assert
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.HandleAsync(_clientMock, _messageEnvelope, _cancellationToken));
        Assert.Equal("Test exception", thrownException.Message);
    }

    [Fact(Timeout = 5000)]
    public async Task HandleAsync_ValidParameters_CallsOnlyUpdateHandler()
    {
        // Act
        await _handler.HandleAsync(_clientMock, _messageEnvelope, _cancellationToken);

        // Assert
        await _updateHandlerMock.Received(1)(Arg.Any<ISignalBotClient>(), Arg.Any<ReceivedMessageEnvelope>(),
            Arg.Any<CancellationToken>());
        await _errorHandlerMock.DidNotReceive()(Arg.Any<ISignalBotClient>(), Arg.Any<Error>(),
            Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task HandleErrorAsync_CallsErrorHandlerWithCorrectParameters()
    {
        // Act
        await _handler.HandleErrorAsync(_clientMock, _error, _cancellationToken);

        // Assert
        await _errorHandlerMock.Received(1)(Arg.Is(_clientMock), Arg.Is(_error), Arg.Is(_cancellationToken));
    }

    [Fact(Timeout = 5000)]
    public async Task HandleErrorAsync_CancelledToken_CallsErrorHandler()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // Act
        await _handler.HandleErrorAsync(_clientMock, _error, cts.Token);

        // Assert
        await _errorHandlerMock.Received(1)(Arg.Is(_clientMock), Arg.Is(_error), Arg.Is(cts.Token));
    }

    [Fact(Timeout = 5000)]
    public async Task HandleErrorAsync_ErrorHandlerThrowsException_PropagatesException()
    {
        // Arrange
        var exception = new InvalidOperationException("Error handler failed");
        _errorHandlerMock(Arg.Any<ISignalBotClient>(), Arg.Any<Error>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(exception);

        // Act & Assert
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.HandleErrorAsync(_clientMock, _error, _cancellationToken));
        Assert.Equal("Error handler failed", thrownException.Message);
    }

    [Fact(Timeout = 5000)]
    public async Task FullLifecycle_BothHandlersWorkIndependently()
    {
        // Arrange
        _updateHandlerMock(Arg.Any<ISignalBotClient>(), Arg.Any<ReceivedMessageEnvelope>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        _errorHandlerMock(Arg.Any<ISignalBotClient>(), Arg.Any<Error>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // Act
        await _handler.HandleAsync(_clientMock, _messageEnvelope, _cancellationToken);
        await _handler.HandleErrorAsync(_clientMock, _error, _cancellationToken);

        // Assert
        await _updateHandlerMock.Received(1)(Arg.Is(_clientMock), Arg.Is(_messageEnvelope), Arg.Is(_cancellationToken));
        await _errorHandlerMock.Received(1)(Arg.Is(_clientMock), Arg.Is(_error), Arg.Is(_cancellationToken));
    }
}