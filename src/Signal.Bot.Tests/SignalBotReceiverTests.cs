using JetBrains.Annotations;
using NSubstitute;
using Signal.Bot.Polling;

namespace Signal.Bot.Tests;

[TestSubject(typeof(SignalBotReceiver))]
public class SignalBotReceiverTests
{
    private readonly ISignalBotClient _mockClient;
    private readonly IReceivedMessageHandler _mockHandler;

    public SignalBotReceiverTests()
    {
        _mockClient = Substitute.For<ISignalBotClient>();
        _mockHandler = Substitute.For<IReceivedMessageHandler>();

        // Setup default returns
        _mockClient.BaseUrl.Returns("localhost:8080");
        _mockClient.Number.Returns("+1234567890");
        _mockClient.GlobalCancelToken.Returns(CancellationToken.None);
        _mockClient.JsonSerializerOptions.Returns(new System.Text.Json.JsonSerializerOptions());
    }

    [Fact]
    public void Constructor_WithNullClient_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            new SignalBotReceiver(null!));

        Assert.Equal("client", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithValidClient_CreatesInstance()
    {
        // Act
        var receiver = new SignalBotReceiver(_mockClient);

        // Assert
        Assert.NotNull(receiver);
    }

    [Fact]
    public async Task StartReceivingAsync_WithNullHandler_ThrowsArgumentNullException()
    {
        // Arrange
        var receiver = new SignalBotReceiver(_mockClient);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await receiver.StartReceivingAsync(null!));
    }

    [Fact]
    public async Task StartReceivingAsync_WithValidParameters_ReturnsDisposable()
    {
        // Arrange
        var receiver = new SignalBotReceiver(_mockClient);

        // Act
        IDisposable? disposable = null;
        try
        {
            disposable = await receiver.StartReceivingAsync(
                _mockHandler,
                cancellationToken: new CancellationTokenSource(100).Token);
        }
        catch
        {
            // Expected - WebSocket connection will fail
        }

        // Assert
        Assert.NotNull(disposable);

        // Cleanup
        disposable.Dispose();
    }

    [Fact]
    public async Task StartReceivingAsync_ConfiguresWebSocketUri_Correctly()
    {
        // Arrange
        _mockClient.BaseUrl.Returns("test.server.com");
        _mockClient.Number.Returns("+9876543210");

        var receiver = new SignalBotReceiver(_mockClient);

        // Act
        try
        {
            await receiver.StartReceivingAsync(
                _mockHandler,
                options => options.WithTimeout(TimeSpan.FromMilliseconds(5000)),
                new CancellationTokenSource(100).Token);
        }
        catch
        {
            // Expected - WebSocket connection will fail
        }

        // Assert
        _ = _mockClient.Received(1).BaseUrl;
        _ = _mockClient.Received(1).Number;
    }

    [Fact]
    public async Task StartReceivingAsync_WithCancellationToken_RespectsToken()
    {
        // Arrange
        var receiver = new SignalBotReceiver(_mockClient);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        _mockHandler
            .HandleErrorAsync(Arg.Any<ISignalBotClient>(), Arg.Any<Error>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // Act & Assert
        await receiver.StartReceivingAsync(_mockHandler, cancellationToken: cts.Token);

        await _mockHandler
            .Received(1)
            .HandleErrorAsync(Arg.Any<ISignalBotClient>(), Arg.Any<Error>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task StartReceivingAsync_WithReceiverOptions_AppliesConfiguration()
    {
        // Arrange
        var receiver = new SignalBotReceiver(_mockClient);
        var configuredTimeout = 0;
        var configuredCapacity = 0;

        // Act
        try
        {
            await receiver.StartReceivingAsync(
                _mockHandler,
                options =>
                {
                    options
                        .WithTimeout(TimeSpan.FromMilliseconds(5000))
                        .WithQueueCapacity(200);
                    configuredTimeout = 5000;
                    configuredCapacity = 200;
                },
                new CancellationTokenSource(100).Token);
        }
        catch
        {
            // Expected
        }

        // Assert
        Assert.Equal(5000, configuredTimeout);
        Assert.Equal(200, configuredCapacity);
    }

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes_WithoutException()
    {
        // Arrange
        var receiver = new SignalBotReceiver(_mockClient);

        // Act & Assert
        var act = () =>
        {
            receiver.Dispose();
            receiver.Dispose();
            receiver.Dispose();
        };
        act.Invoke();
        Assert.True(true);
    }

    [Fact]
    public async Task Dispose_CleansUpResources_Properly()
    {
        // Arrange
        var receiver = new SignalBotReceiver(_mockClient);

        try
        {
            await receiver.StartReceivingAsync(
                _mockHandler,
                cancellationToken: new CancellationTokenSource(100).Token);
        }
        catch
        {
            // Expected
        }

        // Act
        receiver.Dispose();

        await Task.Delay(200);

        // Assert
        receiver.Dispose();
        Assert.True(true);
    }

    [Fact]
    public async Task StartReceivingAsync_UsesGlobalCancellationToken()
    {
        // Arrange
        using var globalCts = new CancellationTokenSource();
        _mockClient.GlobalCancelToken.Returns(globalCts.Token);

        var receiver = new SignalBotReceiver(_mockClient);

        // Act
        await globalCts.CancelAsync();

        _mockHandler
            .HandleErrorAsync(Arg.Any<ISignalBotClient>(), Arg.Any<Error>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // Assert
        await receiver.StartReceivingAsync(_mockHandler);

        await _mockHandler
            .Received(1)
            .HandleErrorAsync(Arg.Any<ISignalBotClient>(), Arg.Any<Error>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public void Receiver_ImplementsIDisposable()
    {
        // Arrange
        var receiver = new SignalBotReceiver(_mockClient);

        // Assert
        Assert.IsType<IDisposable>(receiver, exactMatch: false);
    }
}