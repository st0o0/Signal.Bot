using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Signal.Bot.Polling;
using Signal.Bot.Types;

namespace Signal.Bot.UnitTests.Internal;

public class DefaultReceivedMessageHandlerTests
{
    private readonly Func<ISignalBotClient, ReceivedMessage, CancellationToken, Task> _updateHandlerMock;
    private readonly Func<ISignalBotClient, Error, CancellationToken, Task> _errorHandlerMock;
    private readonly DefaultReceivedMessageHandler _handler;
    private readonly ISignalBotClient _clientMock;
    private readonly ReceivedMessage _message;
    private readonly Error _error;

    public DefaultReceivedMessageHandlerTests()
    {
        _updateHandlerMock = Substitute.For<Func<ISignalBotClient, ReceivedMessage, CancellationToken, Task>>();
        _errorHandlerMock = Substitute.For<Func<ISignalBotClient, Error, CancellationToken, Task>>();

        _handler = new DefaultReceivedMessageHandler(_updateHandlerMock, _errorHandlerMock);

        _clientMock = Substitute.For<ISignalBotClient>();
        _message = new ReceivedMessage { Envelope = new Envelope { SourceNumber = "test", SourceId = Guid.NewGuid() } };
        _error = new Error(null, FailureSource.Failed);
    }

    [Fact(Timeout = 5000)]
    public async Task HandleAsync_CallsUpdateHandlerWithCorrectParameters()
    {
        // Act
        await _handler.HandleAsync(_clientMock, _message, TestContext.Current.CancellationToken);

        // Assert
        await _updateHandlerMock.Received(1)(Arg.Is(_clientMock), Arg.Is(_message), Arg.Is(TestContext.Current.CancellationToken));
    }


    [Fact(Timeout = 5000)]
    public async Task HandleAsync_UpdateHandlerThrowsException_PropagatesException()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        _updateHandlerMock(Arg.Any<ISignalBotClient>(), Arg.Any<ReceivedMessage>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(exception);

        // Act & Assert
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.HandleAsync(_clientMock, _message, TestContext.Current.CancellationToken));
        Assert.Equal("Test exception", thrownException.Message);
    }

    [Fact(Timeout = 5000)]
    public async Task HandleAsync_ValidParameters_CallsOnlyUpdateHandler()
    {
        // Act
        await _handler.HandleAsync(_clientMock, _message, TestContext.Current.CancellationToken);

        // Assert
        await _updateHandlerMock.Received(1)(Arg.Any<ISignalBotClient>(), Arg.Any<ReceivedMessage>(),
            Arg.Any<CancellationToken>());
        await _errorHandlerMock.DidNotReceive()(Arg.Any<ISignalBotClient>(), Arg.Any<Error>(),
            Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task HandleErrorAsync_CallsErrorHandlerWithCorrectParameters()
    {
        // Act
        await _handler.HandleErrorAsync(_clientMock, _error, TestContext.Current.CancellationToken);

        // Assert
        await _errorHandlerMock.Received(1)(Arg.Is(_clientMock), Arg.Is(_error), Arg.Is(TestContext.Current.CancellationToken));
    }

    [Fact(Timeout = 5000)]
    public async Task HandleErrorAsync_CancelledToken_CallsErrorHandler()
    {
        // Arrange
        using var ctsCanceled = new CancellationTokenSource();
        using var cts =  CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken, ctsCanceled.Token);
       
        await ctsCanceled.CancelAsync();

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
            _handler.HandleErrorAsync(_clientMock, _error, TestContext.Current.CancellationToken));
        Assert.Equal("Error handler failed", thrownException.Message);
    }

    [Fact(Timeout = 5000)]
    public async Task FullLifecycle_BothHandlersWorkIndependently()
    {
        // Arrange
        _updateHandlerMock(Arg.Any<ISignalBotClient>(), Arg.Any<ReceivedMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        _errorHandlerMock(Arg.Any<ISignalBotClient>(), Arg.Any<Error>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // Act
        await _handler.HandleAsync(_clientMock, _message, TestContext.Current.CancellationToken);
        await _handler.HandleErrorAsync(_clientMock, _error, TestContext.Current.CancellationToken);

        // Assert
        await _updateHandlerMock.Received(1)(Arg.Is(_clientMock), Arg.Is(_message), Arg.Is(TestContext.Current.CancellationToken));
        await _errorHandlerMock.Received(1)(Arg.Is(_clientMock), Arg.Is(_error), Arg.Is(TestContext.Current.CancellationToken));
    }
}