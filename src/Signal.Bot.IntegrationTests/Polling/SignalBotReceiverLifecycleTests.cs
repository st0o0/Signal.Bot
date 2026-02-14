using System.Text.Json;
using NSubstitute;
using Signal.Bot.IntegrationTests.Utils;
using Signal.Bot.Serialization;
using Signal.Bot.Types;

namespace Signal.Bot.IntegrationTests.Polling;

public class SignalBotReceiverLifecycleTests : ReceiverIntegrationTestBase
{
    [Fact(Timeout = 30000)]
    public async Task Should_Stop_Processing_After_Cancellation()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        var processedAfterCancel = false;
        var firstMessageTcs = new TaskCompletionSource<bool>();

        MockHandler.HandleAsync(
                Arg.Any<ISignalBotClient>(),
                Arg.Any<ReceivedMessageEnvelope>(),
                Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var ct = callInfo.ArgAt<CancellationToken>(2);
                if (ct.IsCancellationRequested)
                {
                    processedAfterCancel = true;
                }
                else
                {
                    firstMessageTcs.SetResult(true);
                }

                return Task.CompletedTask;
            });

        await TestServer.StartAsync();
        var receiver = new SignalBotReceiver(MockClient);
        await receiver.StartReceivingAsync(MockHandler, cancellationToken: cts.Token);

        // Act
        await TestServer.SendMessageAsync(JsonSerializer.Serialize(CreateTestReceivedMessage("Before cancel"),
            JsonBotAPI.Options));
        await firstMessageTcs.Task;

        await cts.CancelAsync();

        await TestServer.SendMessageAsync(JsonSerializer.Serialize(CreateTestReceivedMessage("After cancel"),
            JsonBotAPI.Options));

        // Assert
        Assert.False(processedAfterCancel, "Should not process messages after cancellation");

        await receiver.DisposeAsync();
    }
}
