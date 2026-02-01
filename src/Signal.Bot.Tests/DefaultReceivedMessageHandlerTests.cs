using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Signal.Bot.Polling;
using Signal.Bot.Types;

namespace Signal.Bot.Tests;

public class DefaultReceivedMessageHandlerTests
{
    private readonly Func<ISignalBotClient, ReceivedMessage, CancellationToken, Task> _updateHandlerMock;
    private readonly Func<ISignalBotClient, Error, CancellationToken, Task> _errorHandlerMock;
    private readonly DefaultReceivedMessageHandler _handler;
    private readonly ISignalBotClient _clientMock;
    private readonly ReceivedMessage _message;
    private readonly Error _error;
    private readonly CancellationToken _cancellationToken;

    public DefaultReceivedMessageHandlerTests()
    {
        _updateHandlerMock = Substitute.For<Func<ISignalBotClient, ReceivedMessage, CancellationToken, Task>>();
        _errorHandlerMock = Substitute.For<Func<ISignalBotClient, Error, CancellationToken, Task>>();

        _handler = new DefaultReceivedMessageHandler(_updateHandlerMock, _errorHandlerMock);

        _clientMock = Substitute.For<ISignalBotClient>();
        _message = new ReceivedMessage { Envelope = new Envelope { SourceNumber = "test", SourceId = Guid.NewGuid() } };
        _error = new Error(null, ErrorSource.FatalError);
        _cancellationToken = CancellationToken.None;
    }

    #region HandleAsync Tests

    [Fact]
    public async Task HandleAsync_CallsUpdateHandlerWithCorrectParameters()
    {
        // Act
        await _handler.HandleAsync(_clientMock, _message, _cancellationToken);

        // Assert
        await _updateHandlerMock.Received(1)(Arg.Is(_clientMock), Arg.Is(_message), Arg.Is(_cancellationToken));
    }


    [Fact]
    public async Task HandleAsync_UpdateHandlerThrowsException_PropagatesException()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        _updateHandlerMock(Arg.Any<ISignalBotClient>(), Arg.Any<ReceivedMessage>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(exception);

        // Act & Assert
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.HandleAsync(_clientMock, _message, _cancellationToken));
        Assert.Equal("Test exception", thrownException.Message);
    }

    [Fact]
    public async Task HandleAsync_ValidParameters_CallsOnlyUpdateHandler()
    {
        // Act
        await _handler.HandleAsync(_clientMock, _message, _cancellationToken);

        // Assert
        await _updateHandlerMock.Received(1)(Arg.Any<ISignalBotClient>(), Arg.Any<ReceivedMessage>(),
            Arg.Any<CancellationToken>());
        await _errorHandlerMock.DidNotReceive()(Arg.Any<ISignalBotClient>(), Arg.Any<Error>(),
            Arg.Any<CancellationToken>());
    }

    #endregion

    #region HandleErrorAsync Tests

    [Fact]
    public async Task HandleErrorAsync_CallsErrorHandlerWithCorrectParameters()
    {
        // Act
        await _handler.HandleErrorAsync(_clientMock, _error, _cancellationToken);

        // Assert
        await _errorHandlerMock.Received(1)(Arg.Is(_clientMock), Arg.Is(_error), Arg.Is(_cancellationToken));
    }

    [Fact]
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

    [Fact]
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

    #endregion

    #region Integration Tests

    [Fact]
    public async Task FullLifecycle_BothHandlersWorkIndependently()
    {
        // Arrange
        _updateHandlerMock(Arg.Any<ISignalBotClient>(), Arg.Any<ReceivedMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        _errorHandlerMock(Arg.Any<ISignalBotClient>(), Arg.Any<Error>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // Act
        await _handler.HandleAsync(_clientMock, _message, _cancellationToken);
        await _handler.HandleErrorAsync(_clientMock, _error, _cancellationToken);

        // Assert
        await _updateHandlerMock.Received(1)(Arg.Is(_clientMock), Arg.Is(_message), Arg.Is(_cancellationToken));
        await _errorHandlerMock.Received(1)(Arg.Is(_clientMock), Arg.Is(_error), Arg.Is(_cancellationToken));
    }

    #endregion
}