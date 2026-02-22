using System.Collections.Concurrent;
using System.Text.Json;
using NSubstitute;
using Signal.Bot.IntegrationTests.Utils;
using Signal.Bot.Serialization;
using Signal.Bot.Types;

namespace Signal.Bot.IntegrationTests.Polling;

public class SignalBotReceiverMessageTests : ReceiverIntegrationTestBase
{
    [Fact(Timeout = 30000)]
    public async Task Should_Receive_Text_Message_From_Server()
    {
        // Arrange
        var messageReceivedTcs = new TaskCompletionSource<ReceivedMessage>();

        MockHandler.HandleAsync(
                Arg.Any<ISignalBotClient>(),
                Arg.Any<ReceivedMessage>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask)
            .AndDoes(callInfo => messageReceivedTcs.SetResult(callInfo.ArgAt<ReceivedMessage>(1)));

        await TestServer.StartAsync();
        var receiver = new SignalBotReceiver(MockClient);
        await receiver.StartReceivingAsync(MockHandler, cancellationToken: TestContext.Current.CancellationToken);

        var testMessage = CreateTestReceivedMessage("Hello from server!");

        // Act
        await TestServer.SendMessageAsync(JsonSerializer.Serialize(testMessage, JsonBotAPI.Options));

        // Assert
        var completed = await messageReceivedTcs.Task;
        Assert.NotNull(completed);
        Assert.Equal("Hello from server!", completed.Envelope?.DataMessage?.Message);

        await receiver.DisposeAsync();
    }

    [Fact(Timeout = 20000)]
    public async Task Should_Receive_Binary_Message_From_Server()
    {
        // Arrange
        var messageReceivedTcs = new TaskCompletionSource<ReceivedMessage>();

        MockHandler.HandleAsync(
                Arg.Any<ISignalBotClient>(),
                Arg.Any<ReceivedMessage>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask)
            .AndDoes(callInfo => messageReceivedTcs.SetResult(callInfo.ArgAt<ReceivedMessage>(1)));

        await TestServer.StartAsync();
        var receiver = new SignalBotReceiver(MockClient);
        await receiver.StartReceivingAsync(MockHandler, cancellationToken: TestContext.Current.CancellationToken);

        var testMessage = CreateTestReceivedMessage("Binary message!");
        var json = JsonSerializer.Serialize(testMessage, JsonBotAPI.Options);
        var bytes = System.Text.Encoding.UTF8.GetBytes(json);

        // Act
        await TestServer.SendBinaryMessageAsync(bytes);

        // Assert
        var completed = await messageReceivedTcs.Task;
        Assert.NotNull(completed);
        Assert.Equal("Binary message!", completed.Envelope?.DataMessage?.Message);

        await receiver.DisposeAsync();
    }

    [Fact(Timeout = 20000)]
    public async Task Should_Receive_Multiple_Messages_In_Order()
    {
        // Arrange
        var receivedMessages = new ConcurrentBag<string>();
        var messageCount = 0;
        var allMessagesReceivedTcs = new TaskCompletionSource<bool>();

        MockHandler.HandleAsync(
                Arg.Any<ISignalBotClient>(),
                Arg.Any<ReceivedMessage>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask)
            .AndDoes(callInfo =>
            {
                var msg = callInfo.ArgAt<ReceivedMessage>(1);
                receivedMessages.Add(msg.Envelope?.DataMessage?.Message ?? "");

                if (Interlocked.Increment(ref messageCount) == 5)
                {
                    allMessagesReceivedTcs.SetResult(true);
                }
            });

        await TestServer.StartAsync();
        var receiver = new SignalBotReceiver(MockClient);
        await receiver.StartReceivingAsync(MockHandler, cancellationToken: TestContext.Current.CancellationToken);

        // Act
        for (var i = 1; i <= 5; i++)
        {
            var message = CreateTestReceivedMessage($"Message {i}");
            await TestServer.SendMessageAsync(JsonSerializer.Serialize(message, JsonBotAPI.Options));
        }

        // Assert
        await allMessagesReceivedTcs.Task;
        Assert.Equal(5, receivedMessages.Count);

        for (var i = 1; i <= 5; i++)
        {
            Assert.Contains($"Message {i}", receivedMessages);
        }

        await receiver.DisposeAsync();
    }

    [Fact(Timeout = 20000)]
    public async Task Should_Call_Handler_For_Each_Message()
    {
        // Arrange
        var messageCount = 0;
        const int expectedCount = 3;
        var allHandledTcs = new TaskCompletionSource<bool>();

        MockHandler.HandleAsync(
                Arg.Any<ISignalBotClient>(),
                Arg.Any<ReceivedMessage>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask)
            .AndDoes(_ =>
            {
                if (Interlocked.Increment(ref messageCount) == expectedCount)
                {
                    allHandledTcs.SetResult(true);
                }
            });

        await TestServer.StartAsync();
        var receiver = new SignalBotReceiver(MockClient);
        await receiver.StartReceivingAsync(MockHandler, cancellationToken: TestContext.Current.CancellationToken);

        // Act
        for (var i = 0; i < expectedCount; i++)
        {
            var message = CreateTestReceivedMessage($"Test {i}");
            await TestServer.SendMessageAsync(JsonSerializer.Serialize(message, JsonBotAPI.Options));
        }

        // Assert
        await allHandledTcs.Task;

        await MockHandler.Received(expectedCount).HandleAsync(
            MockClient,
            Arg.Any<ReceivedMessage>(),
            Arg.Any<CancellationToken>());

        await receiver.DisposeAsync();
    }

    [Fact(Timeout = 20000)]
    public async Task Should_Handle_Group_Message()
    {
        // Arrange
        var messageReceivedTcs = new TaskCompletionSource<ReceivedMessage>();

        MockHandler.HandleAsync(
                Arg.Any<ISignalBotClient>(),
                Arg.Any<ReceivedMessage>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask)
            .AndDoes(callInfo => messageReceivedTcs.SetResult(callInfo.ArgAt<ReceivedMessage>(1)));

        await TestServer.StartAsync();
        var receiver = new SignalBotReceiver(MockClient);
        await receiver.StartReceivingAsync(MockHandler, cancellationToken: TestContext.Current.CancellationToken);

        var groupMessage = CreateTestGroupMessage("Hello group!", "group-123", "Test Group");

        // Act
        await TestServer.SendMessageAsync(JsonSerializer.Serialize(groupMessage, JsonBotAPI.Options));

        // Assert
        var completed = await messageReceivedTcs.Task;
        Assert.NotNull(completed);
        Assert.Equal("Hello group!", completed.Envelope?.DataMessage?.Message);
        Assert.Equal("Test Group", completed.Envelope?.DataMessage?.GroupV2?.Name);
        Assert.Equal("group-123", completed.Envelope?.DataMessage?.GroupV2?.Id);

        await receiver.DisposeAsync();
    }

    [Fact(Timeout = 20000)]
    public async Task Should_Handle_Message_With_Attachment()
    {
        // Arrange
        var messageReceivedTcs = new TaskCompletionSource<ReceivedMessage>();

        MockHandler.HandleAsync(
                Arg.Any<ISignalBotClient>(),
                Arg.Any<ReceivedMessage>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask)
            .AndDoes(callInfo => messageReceivedTcs.SetResult(callInfo.ArgAt<ReceivedMessage>(1)));

        await TestServer.StartAsync();
        var receiver = new SignalBotReceiver(MockClient);
        await receiver.StartReceivingAsync(MockHandler, cancellationToken: TestContext.Current.CancellationToken);

        var attachmentMessage = CreateTestMessageWithAttachment("Check this file", "document.pdf", "application/pdf");

        // Act
        await TestServer.SendMessageAsync(JsonSerializer.Serialize(attachmentMessage, JsonBotAPI.Options));

        // Assert
        var completed = await messageReceivedTcs.Task;
        Assert.NotNull(completed);
        Assert.NotNull(completed.Envelope?.DataMessage?.Attachments);
        Assert.Single(completed.Envelope.DataMessage.Attachments);
        Assert.Equal("document.pdf", completed.Envelope.DataMessage.Attachments[0].Filename);
        Assert.Equal("application/pdf", completed.Envelope.DataMessage.Attachments[0].ContentType);

        await receiver.DisposeAsync();
    }

    [Fact(Timeout = 20000)]
    public async Task Should_Handle_Message_With_Multiple_Attachments()
    {
        // Arrange
        var messageReceivedTcs = new TaskCompletionSource<ReceivedMessage>();

        MockHandler.HandleAsync(
                Arg.Any<ISignalBotClient>(),
                Arg.Any<ReceivedMessage>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask)
            .AndDoes(callInfo => messageReceivedTcs.SetResult(callInfo.ArgAt<ReceivedMessage>(1)));

        await TestServer.StartAsync();
        var receiver = new SignalBotReceiver(MockClient);
        await receiver.StartReceivingAsync(MockHandler, cancellationToken: TestContext.Current.CancellationToken);

        var message = CreateTestMessageWithMultipleAttachments("Files attached", 3);

        // Act
        await TestServer.SendMessageAsync(JsonSerializer.Serialize(message, JsonBotAPI.Options));

        // Assert
        var completed = await messageReceivedTcs.Task;
        Assert.NotNull(completed.Envelope?.DataMessage?.Attachments);
        Assert.Equal(3, completed.Envelope.DataMessage.Attachments.Count);

        await receiver.DisposeAsync();
    }
}
