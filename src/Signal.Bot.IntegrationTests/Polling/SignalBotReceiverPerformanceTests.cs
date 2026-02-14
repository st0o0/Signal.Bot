using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;
using NSubstitute;
using Signal.Bot.IntegrationTests.Utils;
using Signal.Bot.Serialization;
using Signal.Bot.Types;

namespace Signal.Bot.IntegrationTests.Polling;

public class SignalBotReceiverPerformanceTests : ReceiverIntegrationTestBase
{
    [Fact(Timeout = 30000)]
    [Trait("Speed", "Slow")]
    public async Task Should_Handle_High_Volume_Sequential_Messages()
    {
        // Arrange
        const int messageCount = 1000;
        var receivedCount = 0;
        var allReceivedTcs = new TaskCompletionSource<bool>();

        MockHandler.HandleAsync(
                Arg.Any<ISignalBotClient>(),
                Arg.Any<ReceivedMessageEnvelope>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask)
            .AndDoes(_ =>
            {
                if (Interlocked.Increment(ref receivedCount) == messageCount)
                {
                    allReceivedTcs.SetResult(true);
                }
            });

        await TestServer.StartAsync();
        var receiver = new SignalBotReceiver(MockClient);
        await receiver.StartReceivingAsync(MockHandler, cancellationToken: TestContext.Current.CancellationToken);

        var stopwatch = Stopwatch.StartNew();

        // Act
        for (var i = 0; i < messageCount; i++)
        {
            var message = CreateTestReceivedMessage($"Message {i}");
            await TestServer.SendMessageAsync(JsonSerializer.Serialize(message, JsonBotAPI.Options));
        }

        // Assert
        await allReceivedTcs.Task;
        stopwatch.Stop();

        Assert.Equal(messageCount, receivedCount);
        var messagesPerSecond = messageCount / stopwatch.Elapsed.TotalSeconds;
        Console.WriteLine(
            $"Processed {messageCount} messages in {stopwatch.ElapsedMilliseconds}ms ({messagesPerSecond:F2} msg/s)");

        await receiver.DisposeAsync();
    }

    [Fact(Timeout = 30000)]
    [Trait("Speed", "Slow")]
    public async Task Should_Handle_Large_Message_Payloads()
    {
        // Arrange
        const int messageCount = 100;
        const int messageSize = 50_000;
        var receivedCount = 0;
        var allReceivedTcs = new TaskCompletionSource<bool>();

        MockHandler.HandleAsync(
                Arg.Any<ISignalBotClient>(),
                Arg.Any<ReceivedMessageEnvelope>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask)
            .AndDoes(_ =>
            {
                if (Interlocked.Increment(ref receivedCount) == messageCount)
                {
                    allReceivedTcs.SetResult(true);
                }
            });

        await TestServer.StartAsync();
        var receiver = new SignalBotReceiver(MockClient);
        await receiver.StartReceivingAsync(MockHandler, cancellationToken: TestContext.Current.CancellationToken);

        var stopwatch = Stopwatch.StartNew();

        // Act
        for (var i = 0; i < messageCount; i++)
        {
            var largeText = new string('X', messageSize);
            var message = CreateTestReceivedMessage(largeText);
            await TestServer.SendMessageAsync(JsonSerializer.Serialize(message, JsonBotAPI.Options));
        }


        // Assert
        await allReceivedTcs.Task;
        stopwatch.Stop();

        Assert.Equal(messageCount, receivedCount);
        var totalMB = (messageCount * messageSize) / 1024.0 / 1024.0;
        var mbPerSecond = totalMB / stopwatch.Elapsed.TotalSeconds;
        Console.WriteLine($"Processed {totalMB:F2} MB in {stopwatch.ElapsedMilliseconds}ms ({mbPerSecond:F2} MB/s)");

        await receiver.DisposeAsync();
    }

    [Fact(Timeout = 30000)]
    [Trait("Speed", "Slow")]
    public async Task Should_Maintain_Message_Order_Under_Load()
    {
        // Arrange
        const int messageCount = 500;
        var receivedMessages = new ConcurrentQueue<int>();
        var receivedCount = 0;
        var allReceivedTcs = new TaskCompletionSource<bool>();

        MockHandler.HandleAsync(
                Arg.Any<ISignalBotClient>(),
                Arg.Any<ReceivedMessageEnvelope>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask)
            .AndDoes(callInfo =>
            {
                var msg = callInfo.ArgAt<ReceivedMessageEnvelope>(1);
                var messageText = msg.Envelope?.DataMessage?.Message ?? "";
                var messageNumber = int.Parse(messageText.Replace("Message ", ""));
                receivedMessages.Enqueue(messageNumber);

                if (Interlocked.Increment(ref receivedCount) == messageCount)
                {
                    allReceivedTcs.SetResult(true);
                }
            });

        await TestServer.StartAsync();
        var receiver = new SignalBotReceiver(MockClient);
        await receiver.StartReceivingAsync(MockHandler, cancellationToken: TestContext.Current.CancellationToken);

        // Act
        for (var i = 0; i < messageCount; i++)
        {
            var message = CreateTestReceivedMessage($"Message {i}");
            await TestServer.SendMessageAsync(JsonSerializer.Serialize(message, JsonBotAPI.Options));
        }

        // Assert
        await allReceivedTcs.Task;
        Assert.Equal(messageCount, receivedMessages.Count);

        var orderedList = receivedMessages.ToList();
        for (var i = 0; i < messageCount; i++)
        {
            Assert.Equal(i, orderedList[i]);
        }

        await receiver.DisposeAsync();
    }

    [Fact(Timeout = 30000)]
    [Trait("Speed", "Slow")]
    public async Task Should_Handle_Mixed_Message_Types_At_Scale()
    {
        // Arrange
        const int messagesPerType = 100;
        var dataMessageCount = 0;
        var receiptMessageCount = 0;
        var typingMessageCount = 0;
        var syncMessageCount = 0;
        var allReceivedTcs = new TaskCompletionSource<bool>();

        MockHandler.HandleAsync(
                Arg.Any<ISignalBotClient>(),
                Arg.Any<ReceivedMessageEnvelope>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask)
            .AndDoes(callInfo =>
            {
                var msg = callInfo.ArgAt<ReceivedMessageEnvelope>(1);
                if (msg.Envelope?.DataMessage != null)
                {
                    Interlocked.Increment(ref dataMessageCount);
                }

                if (msg.Envelope?.ReceiptMessage != null)
                {
                    Interlocked.Increment(ref receiptMessageCount);
                }

                if (msg.Envelope?.TypingMessage != null)
                {
                    Interlocked.Increment(ref typingMessageCount);
                }

                if (msg.Envelope?.SyncMessage != null)
                {
                    Interlocked.Increment(ref syncMessageCount);
                }

                var total = dataMessageCount + receiptMessageCount + typingMessageCount + syncMessageCount;
                if (total == messagesPerType * 4)
                {
                    allReceivedTcs.SetResult(true);
                }
            });

        await TestServer.StartAsync();
        var receiver = new SignalBotReceiver(MockClient);
        await receiver.StartReceivingAsync(MockHandler, cancellationToken: TestContext.Current.CancellationToken);

        // Act
        var messages = new List<ReceivedMessageEnvelope>();
        for (var i = 0; i < messagesPerType; i++)
        {
            messages.Add(CreateTestReceivedMessage($"Data {i}"));
            messages.Add(CreateTestReceiptMessage());
            messages.Add(CreateTestTypingMessage());
            messages.Add(CreateTestSyncMessage());
        }

        // Randomize order
        var random = new Random(42);
        messages = messages.OrderBy(_ => random.Next()).ToList();

        foreach (var message in messages)
        {
            await TestServer.SendMessageAsync(JsonSerializer.Serialize(message, JsonBotAPI.Options));
        }

        // Assert
        await allReceivedTcs.Task;
        Assert.Equal(messagesPerType, dataMessageCount);
        Assert.Equal(messagesPerType, receiptMessageCount);
        Assert.Equal(messagesPerType, typingMessageCount);
        Assert.Equal(messagesPerType, syncMessageCount);

        await receiver.DisposeAsync();
    }
}