using NSubstitute;
using Signal.Bot.IntegrationTests.Utils;
using Signal.Bot.Polling;

namespace Signal.Bot.IntegrationTests.Polling;

public class SignalBotReceiverConnectionTests : ReceiverIntegrationTestBase
{
    [Fact(Timeout = 10000)]
    public async Task Should_Connect_To_WebSocket_Server()
    {
        // Arrange
        var connectionTcs = new TaskCompletionSource<bool>();
        TestServer.OnClientConnected += () =>
        {
            connectionTcs.SetResult(true);
            return Task.CompletedTask;
        };

        await TestServer.StartAsync();
        var receiver = new SignalBotReceiver(MockClient);

        // Act
        await receiver.StartReceivingAsync(MockHandler, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        var connected = await connectionTcs.Task;
        Assert.True(connected, "Client should connect to server");

        await receiver.DisposeAsync();
    }

    [Fact(Timeout = 10000)]
    public async Task Should_Handle_Server_Disconnect()
    {
        // Arrange
        var disconnectTcs = new TaskCompletionSource<bool>();

        MockHandler.HandleErrorAsync(
                Arg.Any<ISignalBotClient>(),
                Arg.Is<Error>(e => e.ErrorType == ErrorType.DisconnectionHappened),
                Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask)
            .AndDoes(_ => disconnectTcs.SetResult(true));

        await TestServer.StartAsync();
        var receiver = new SignalBotReceiver(MockClient);
        await receiver.StartReceivingAsync(MockHandler, cancellationToken: TestContext.Current.CancellationToken);

        // Act
        await TestServer.DisconnectAsync();

        // Assert
        var completed = await disconnectTcs.Task.WaitAsync(
            TimeSpan.FromSeconds(5),
            TestContext.Current.CancellationToken);
        Assert.True(completed, "Should handle disconnect");

        await receiver.DisposeAsync();
    }

    [Fact(Timeout = 15000)]
    public async Task Should_Dispose_Cleanly()
    {
        // Arrange
        await TestServer.StartAsync();
        var receiver = new SignalBotReceiver(MockClient);
        await receiver.StartReceivingAsync(MockHandler, cancellationToken: TestContext.Current.CancellationToken);

        // Act & Assert
        await receiver.DisposeAsync();
        Assert.True(true, "Dispose completed without exception");
    }
}
