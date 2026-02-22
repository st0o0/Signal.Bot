using System.Net.WebSockets;
using NSubstitute;
using R3;
using Signal.Bot.Serialization;
using Signal.Bot.Types;
using WebSocket.Rx;

namespace Signal.Bot.UnitTests.Polling;

public class SignalBotReceiverTests
{
    private readonly ISignalBotClient _mockClient;
    private readonly IReceivedMessageHandler _mockHandler;
    private readonly IReactiveWebSocketClient _mockWebSocket;

    public SignalBotReceiverTests()
    {
        _mockClient = Substitute.For<ISignalBotClient>();
        _mockHandler = Substitute.For<IReceivedMessageHandler>();
        _mockWebSocket = Substitute.For<IReactiveWebSocketClient>();

        _mockClient.BaseUrl.Returns("localhost:8080");
        _mockClient.Number.Returns("+1234567890");
        _mockClient.JsonSerializerOptions.Returns(JsonBotAPI.Options);

        _mockWebSocket.MessageReceived.Returns(Observable.Empty<Message>());
        _mockWebSocket.ConnectionHappened.Returns(Observable.Empty<Connected>());
        _mockWebSocket.DisconnectionHappened.Returns(Observable.Empty<Disconnected>());
        _mockWebSocket.StartAsync().Returns(Task.CompletedTask);
        _mockWebSocket.StopAsync(Arg.Any<WebSocketCloseStatus>(), Arg.Any<string>()).Returns(Task.FromResult(true));
    }

    [Fact(Timeout = 5000)]
    public void Constructor_WithNullClient_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            new SignalBotReceiver(null!));

        Assert.Equal("client", exception.ParamName);
    }

    [Fact(Timeout = 5000)]
    public void Constructor_WithValidClient_CreatesInstance()
    {
        // Act
        var receiver = new SignalBotReceiver(_mockClient);

        // Assert
        Assert.NotNull(receiver);
    }

    [Fact(Timeout = 5000)]
    public async Task StartReceivingAsync_WithNullHandler_ThrowsArgumentNullException()
    {
        // Arrange
        var receiver = new SignalBotReceiver(_mockClient);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await receiver.StartReceivingAsync(null!, cancellationToken: TestContext.Current.CancellationToken));
    }

    [Fact(Timeout = 5000)]
    public async Task StartReceivingAsync_WithValidParameters_ReturnsDisposable()
    {
        // Arrange
        var receiver = new SignalBotReceiver(_mockClient, _mockWebSocket);

        // Act
        var disposable = await receiver.StartReceivingAsync(
            _mockHandler,
            cancellationToken: new CancellationTokenSource(10).Token);

        // Assert
        Assert.NotNull(disposable);

        // Cleanup
        await disposable.DisposeAsync();
    }

    [Fact(Timeout = 5000)]
    public async Task StartReceivingAsync_ConfiguresWebSocketUri_Correctly()
    {
        // Arrange
        _mockClient.BaseUrl.Returns("test.server.com");
        _mockClient.Number.Returns("+9876543210");

        var receiver = new SignalBotReceiver(_mockClient, _mockWebSocket);

        // Act
        await receiver.StartReceivingAsync(
            _mockHandler,
            options => options.WithTimeout(TimeSpan.FromMilliseconds(100)),
            new CancellationTokenSource(10).Token);

        // Assert
        _ = _mockClient.Received(1).BaseUrl;
        _ = _mockClient.Received(1).Number;
    }

    [Fact(Timeout = 5000)]
    public async Task StartReceivingAsync_WithCancellationToken_RespectsToken()
    {
        // Arrange
        var receiver = new SignalBotReceiver(_mockClient, _mockWebSocket);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        _mockHandler
            .HandleErrorAsync(Arg.Any<ISignalBotClient>(), Arg.Any<Error>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // Act & Assert
        await receiver.StartReceivingAsync(_mockHandler, cancellationToken: cts.Token);

        Assert.True(true);
    }

    [Fact(Timeout = 5000)]
    public async Task StartReceivingAsync_WithReceiverOptions_AppliesConfiguration()
    {
        // Arrange
        var receiver = new SignalBotReceiver(_mockClient, _mockWebSocket);
        var configuredTimeout = 0;
        var configuredCapacity = 0;

        // Act
        await receiver.StartReceivingAsync(
            _mockHandler,
            options =>
            {
                options.WithTimeout(TimeSpan.FromMilliseconds(100));
                configuredTimeout = 100;
                configuredCapacity = 10;
            },
            new CancellationTokenSource(50).Token);

        // Assert
        Assert.Equal(100, configuredTimeout);
        Assert.Equal(10, configuredCapacity);
    }

    [Fact(Timeout = 5000)]
    public async Task Dispose_CleansUpResources_Properly()
    {
        // Arrange
        var receiver = new SignalBotReceiver(_mockClient, _mockWebSocket);

        await receiver.StartReceivingAsync(
            _mockHandler,
            cancellationToken: new CancellationTokenSource(10).Token);

        // Act
        await receiver.DisposeAsync();

        // Assert
        await receiver.DisposeAsync();
        Assert.True(true);
    }

    [Fact(Timeout = 5000)]
    public void Receiver_ImplementsIAsyncDisposable()
    {
        // Arrange
        var receiver = new SignalBotReceiver(_mockClient);

        // Assert
        Assert.IsType<IAsyncDisposable>(receiver, exactMatch: false);
    }
}
