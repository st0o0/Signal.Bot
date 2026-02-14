using System.Collections.Concurrent;
using System.Text.Json;
using NSubstitute;
using Signal.Bot.IntegrationTests.Utils;
using Signal.Bot.Serialization;
using Signal.Bot.Types;

namespace Signal.Bot.IntegrationTests.Polling;

public class SignalBotReceiverFilterTests : ReceiverIntegrationTestBase
{
    [Fact(Timeout = 10000)]
    public async Task Should_Filter_Receipt_Messages_When_IgnoreReceipt_Is_True()
    {
        // Arrange
        var receivedMessages = new ConcurrentBag<ReceivedMessageEnvelope>();
        var dataMessageTcs = new TaskCompletionSource<bool>();

        MockHandler.HandleAsync(
                Arg.Any<ISignalBotClient>(),
                Arg.Any<ReceivedMessageEnvelope>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask)
            .AndDoes(callInfo =>
            {
                var msg = callInfo.ArgAt<ReceivedMessageEnvelope>(1);
                receivedMessages.Add(msg);
                if (msg.Envelope?.DataMessage != null)
                {
                    dataMessageTcs.SetResult(true);
                }
            });

        await TestServer.StartAsync();
        var receiver = new SignalBotReceiver(MockClient);
        await receiver.StartReceivingAsync(
            MockHandler,
            options => options.WithIgnoreReceipt(),
            cancellationToken: TestContext.Current.CancellationToken);

        // Act
        await TestServer.SendMessageAsync(JsonSerializer.Serialize(CreateTestReceiptMessage(), JsonBotAPI.Options));
        await TestServer.SendMessageAsync(JsonSerializer.Serialize(CreateTestReceivedMessage("Normal message"),
            JsonBotAPI.Options));

        // Assert
        await dataMessageTcs.Task;
        Assert.Single(receivedMessages);
        Assert.NotNull(receivedMessages.First().Envelope?.DataMessage);
        Assert.Null(receivedMessages.First().Envelope?.ReceiptMessage);

        await receiver.DisposeAsync();
    }

    [Fact(Timeout = 10000)]
    public async Task Should_Filter_Typing_Messages_When_IgnoreTyping_Is_True()
    {
        // Arrange
        var receivedMessages = new ConcurrentBag<ReceivedMessageEnvelope>();
        var dataMessageTcs = new TaskCompletionSource<bool>();

        MockHandler.HandleAsync(
                Arg.Any<ISignalBotClient>(),
                Arg.Any<ReceivedMessageEnvelope>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask)
            .AndDoes(callInfo =>
            {
                var msg = callInfo.ArgAt<ReceivedMessageEnvelope>(1);
                receivedMessages.Add(msg);
                if (msg.Envelope?.DataMessage != null)
                {
                    dataMessageTcs.SetResult(true);
                }
            });

        await TestServer.StartAsync();
        var receiver = new SignalBotReceiver(MockClient);
        await receiver.StartReceivingAsync(
            MockHandler,
            options => options.WithIgnoreTyping(),
            cancellationToken: TestContext.Current.CancellationToken);

        // Act
        await TestServer.SendMessageAsync(JsonSerializer.Serialize(CreateTestTypingMessage(), JsonBotAPI.Options));
        await TestServer.SendMessageAsync(JsonSerializer.Serialize(CreateTestReceivedMessage("Normal message"),
            JsonBotAPI.Options));

        // Assert
        await dataMessageTcs.Task;
        Assert.Single(receivedMessages);
        Assert.NotNull(receivedMessages.First().Envelope?.DataMessage);
        Assert.Null(receivedMessages.First().Envelope?.TypingMessage);

        await receiver.DisposeAsync();
    }

    [Fact(Timeout = 10000)]
    public async Task Should_Filter_Sync_Messages_When_IgnoreSync_Is_True()
    {
        // Arrange
        var receivedMessages = new ConcurrentBag<ReceivedMessageEnvelope>();
        var dataMessageTcs = new TaskCompletionSource<bool>();

        MockHandler.HandleAsync(
                Arg.Any<ISignalBotClient>(),
                Arg.Any<ReceivedMessageEnvelope>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask)
            .AndDoes(callInfo =>
            {
                var msg = callInfo.ArgAt<ReceivedMessageEnvelope>(1);
                receivedMessages.Add(msg);
                if (msg.Envelope?.DataMessage != null)
                {
                    dataMessageTcs.SetResult(true);
                }
            });

        await TestServer.StartAsync();
        var receiver = new SignalBotReceiver(MockClient);
        await receiver.StartReceivingAsync(
            MockHandler,
            options => options.WithIgnoreSync(),
            cancellationToken: TestContext.Current.CancellationToken);

        // Act
        await TestServer.SendMessageAsync(JsonSerializer.Serialize(CreateTestSyncMessage(), JsonBotAPI.Options));
        await TestServer.SendMessageAsync(JsonSerializer.Serialize(CreateTestReceivedMessage("Normal message"),
            JsonBotAPI.Options));

        // Assert
        await dataMessageTcs.Task;
        Assert.Single(receivedMessages);
        Assert.NotNull(receivedMessages.First().Envelope?.DataMessage);
        Assert.Null(receivedMessages.First().Envelope?.SyncMessage);

        await receiver.DisposeAsync();
    }

    [Fact(Timeout = 10000)]
    public async Task Should_Filter_Multiple_Message_Types_When_All_Ignore_Options_Are_Enabled()
    {
        // Arrange
        var receivedMessages = new ConcurrentBag<ReceivedMessageEnvelope>();
        var dataMessageTcs = new TaskCompletionSource<bool>();

        MockHandler.HandleAsync(
                Arg.Any<ISignalBotClient>(),
                Arg.Any<ReceivedMessageEnvelope>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask)
            .AndDoes(callInfo =>
            {
                var msg = callInfo.ArgAt<ReceivedMessageEnvelope>(1);
                receivedMessages.Add(msg);
                if (msg.Envelope?.DataMessage != null)
                {
                    dataMessageTcs.SetResult(true);
                }
            });

        await TestServer.StartAsync();
        var receiver = new SignalBotReceiver(MockClient);
        await receiver.StartReceivingAsync(
            MockHandler,
            options => options.WithIgnoreTyping().WithIgnoreSync().WithIgnoreReceipt(),
            cancellationToken: TestContext.Current.CancellationToken);

        // Act
        await TestServer.SendMessageAsync(JsonSerializer.Serialize(CreateTestReceiptMessage(), JsonBotAPI.Options));
        await TestServer.SendMessageAsync(JsonSerializer.Serialize(CreateTestTypingMessage(), JsonBotAPI.Options));
        await TestServer.SendMessageAsync(JsonSerializer.Serialize(CreateTestSyncMessage(), JsonBotAPI.Options));
        await TestServer.SendMessageAsync(JsonSerializer.Serialize(CreateTestReceivedMessage("Data message"),
            JsonBotAPI.Options));

        // Assert
        await dataMessageTcs.Task;
        Assert.Single(receivedMessages);
        Assert.NotNull(receivedMessages.First().Envelope?.DataMessage);
        Assert.Equal("Data message", receivedMessages.First().Envelope?.DataMessage?.Message);

        await receiver.DisposeAsync();
    }

    [Fact(Timeout = 10000)]
    public async Task Should_Not_Filter_Messages_When_All_Ignore_Options_Are_Disabled()
    {
        // Arrange
        var messageCount = 0;
        var allMessagesReceivedTcs = new TaskCompletionSource<bool>();

        MockHandler.HandleAsync(
                Arg.Any<ISignalBotClient>(),
                Arg.Any<ReceivedMessageEnvelope>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask)
            .AndDoes(_ =>
            {
                if (Interlocked.Increment(ref messageCount) == 4)
                {
                    allMessagesReceivedTcs.SetResult(true);
                }
            });

        await TestServer.StartAsync();
        var receiver = new SignalBotReceiver(MockClient);
        await receiver.StartReceivingAsync(
            MockHandler,
            builder => builder.WithIgnoreReceipt(false).WithIgnoreSync(false).WithIgnoreTyping(false),
            cancellationToken: TestContext.Current.CancellationToken);

        // Act
        await TestServer.SendMessageAsync(JsonSerializer.Serialize(CreateTestReceiptMessage(), JsonBotAPI.Options));
        await TestServer.SendMessageAsync(JsonSerializer.Serialize(CreateTestTypingMessage(), JsonBotAPI.Options));
        await TestServer.SendMessageAsync(JsonSerializer.Serialize(CreateTestSyncMessage(), JsonBotAPI.Options));
        await TestServer.SendMessageAsync(JsonSerializer.Serialize(CreateTestReceivedMessage("Data message"),
            JsonBotAPI.Options));

        // Assert
        await allMessagesReceivedTcs.Task;
        Assert.Equal(4, messageCount);

        await receiver.DisposeAsync();
    }
}