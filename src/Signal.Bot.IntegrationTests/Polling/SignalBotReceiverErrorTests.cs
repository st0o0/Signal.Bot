using System.Text.Json;
using NSubstitute;
using Signal.Bot.IntegrationTests.Utils;
using Signal.Bot.Serialization;
using Signal.Bot.Types;

namespace Signal.Bot.IntegrationTests.Polling;

public class SignalBotReceiverErrorTests : ReceiverIntegrationTestBase
{
    [Fact(Timeout = 20000)]
    public async Task Should_Handle_Exception_In_Handler()
    {
        // Arrange
        var exceptionHandledTcs = new TaskCompletionSource<bool>();
        var expectedException = new InvalidOperationException("Handler failed!");

        MockHandler.HandleAsync(
                Arg.Any<ISignalBotClient>(),
                Arg.Any<ReceivedMessage>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromException(expectedException));

        MockHandler.HandleErrorAsync(
                Arg.Any<ISignalBotClient>(),
                Arg.Is<Error>(e =>
                    e.Exception == expectedException &&
                    e.ErrorType == ErrorType.MessageReceived),
                Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask)
            .AndDoes(_ => exceptionHandledTcs.SetResult(true));

        await TestServer.StartAsync();
        var receiver = new SignalBotReceiver(MockClient);
        await receiver.StartReceivingAsync(MockHandler, cancellationToken: TestContext.Current.CancellationToken);

        var message = CreateTestReceivedMessage("This will cause error");
        var json = JsonSerializer.Serialize(message, JsonBotAPI.Options);

        // Act
        await TestServer.SendMessageAsync(json);

        // Assert
        var completed = await exceptionHandledTcs.Task;
        Assert.True(completed, "Error should be handled");

        await MockHandler.Received(1).HandleErrorAsync(
            MockClient,
            Arg.Is<Error>(e => e.Exception == expectedException),
            Arg.Any<CancellationToken>());

        await receiver.DisposeAsync();
    }

    [Fact(Timeout = 10000)]
    public async Task Should_Continue_After_Handler_Exception()
    {
        // Arrange
        var callCount = 0;
        var secondMessageTcs = new TaskCompletionSource<bool>();

        MockHandler.HandleAsync(
                Arg.Any<ISignalBotClient>(),
                Arg.Any<ReceivedMessage>(),
                Arg.Any<CancellationToken>())
            .Returns(_ =>
            {
                var count = Interlocked.Increment(ref callCount);
                if (count == 1)
                {
                    throw new Exception("First message fails");
                }

                if (count == 2)
                {
                    secondMessageTcs.SetResult(true);
                }

                return Task.CompletedTask;
            });

        MockHandler.HandleErrorAsync(
                Arg.Any<ISignalBotClient>(),
                Arg.Any<Error>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        await TestServer.StartAsync();
        var receiver = new SignalBotReceiver(MockClient);
        await receiver.StartReceivingAsync(MockHandler, cancellationToken: TestContext.Current.CancellationToken);

        // Act
        await TestServer.SendMessageAsync(JsonSerializer.Serialize(CreateTestReceivedMessage("First"),
            JsonBotAPI.Options));
        await TestServer.SendMessageAsync(JsonSerializer.Serialize(CreateTestReceivedMessage("Second"),
            JsonBotAPI.Options));

        // Assert
        var completed = await secondMessageTcs.Task;
        Assert.True(completed, "Should process second message after error");

        await receiver.DisposeAsync();
    }
}
