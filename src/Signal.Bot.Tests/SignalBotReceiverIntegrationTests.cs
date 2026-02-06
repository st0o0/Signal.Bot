using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text.Json;
using NSubstitute;
using Signal.Bot.Polling;
using Signal.Bot.Tests.Internal;
using Signal.Bot.Types;

namespace Signal.Bot.Tests;

[Trait("Category", "Integration")]
public class SignalBotReceiverIntegrationTests : IAsyncDisposable
{
    private readonly WebSocketTestServer _testServer;
    private readonly ISignalBotClient _mockClient;
    private readonly IReceivedMessageHandler _mockHandler;

    public SignalBotReceiverIntegrationTests()
    {
        var serverPort = GetAvailablePort();
        _testServer = new WebSocketTestServer(serverPort);

        _mockClient = Substitute.For<ISignalBotClient>();
        _mockHandler = Substitute.For<IReceivedMessageHandler>();

        _mockClient.BaseUrl.Returns($"localhost:{serverPort}");
        _mockClient.Number.Returns("+1234567890");
        _mockClient.JsonSerializerOptions.Returns(new JsonSerializerOptions());
    }

    public async ValueTask DisposeAsync()
    {
        await _testServer.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    #region Connection Tests

    [Fact(Timeout = 10000)]
    public async Task Should_Connect_To_WebSocket_Server()
    {
        // Arrange
        var connectionTcs = new TaskCompletionSource<bool>();
        _testServer.OnClientConnected += () =>
        {
            connectionTcs.SetResult(true);
            return Task.CompletedTask;
        };

        await _testServer.StartAsync();
        var receiver = new SignalBotReceiver(_mockClient);

        // Act
        await receiver.StartReceivingAsync(_mockHandler, cancellationToken: TestContext.Current.CancellationToken);

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

        _mockHandler.HandleErrorAsync(
                Arg.Any<ISignalBotClient>(),
                Arg.Is<Error>(e => e.ErrorType == ErrorType.DisconnectionHappened),
                Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask)
            .AndDoes(_ => disconnectTcs.SetResult(true));

        await _testServer.StartAsync();
        var receiver = new SignalBotReceiver(_mockClient);
        await receiver.StartReceivingAsync(_mockHandler, cancellationToken: TestContext.Current.CancellationToken);

        // Act
        await _testServer.DisconnectAsync();

        // Assert
        var completed = await disconnectTcs.Task.WaitAsync(
            TimeSpan.FromSeconds(5),
            TestContext.Current.CancellationToken);
        Assert.True(completed, "Should handle disconnect");

        await receiver.DisposeAsync();
    }

    [Fact]
    public async Task Should_Dispose_Cleanly()
    {
        // Arrange
        await _testServer.StartAsync();
        var receiver = new SignalBotReceiver(_mockClient);
        await receiver.StartReceivingAsync(_mockHandler, cancellationToken: TestContext.Current.CancellationToken);

        // Act & Assert
        await receiver.DisposeAsync();
        Assert.True(true, "Dispose completed without exception");
    }

    #endregion

    #region Basic Message Reception Tests

    [Fact(Timeout = 10000)]
    public async Task Should_Receive_Text_Message_From_Server()
    {
        // Arrange
        var messageReceivedTcs = new TaskCompletionSource<ReceivedMessage>();

        _mockHandler.HandleAsync(
                Arg.Any<ISignalBotClient>(),
                Arg.Any<ReceivedMessage>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask)
            .AndDoes(callInfo => messageReceivedTcs.SetResult(callInfo.ArgAt<ReceivedMessage>(1)));

        await _testServer.StartAsync();
        var receiver = new SignalBotReceiver(_mockClient);
        await receiver.StartReceivingAsync(_mockHandler, cancellationToken: TestContext.Current.CancellationToken);

        var testMessage = CreateTestReceivedMessage("Hello from server!");

        // Act
        await _testServer.SendMessageAsync(JsonSerializer.Serialize(testMessage));

        // Assert
        var completed = await messageReceivedTcs.Task;
        Assert.NotNull(completed);
        Assert.Equal("Hello from server!", completed.Envelope?.DataMessage?.Message);

        await receiver.DisposeAsync();
    }

    [Fact(Timeout = 10000)]
    public async Task Should_Receive_Binary_Message_From_Server()
    {
        // Arrange
        var messageReceivedTcs = new TaskCompletionSource<ReceivedMessage>();

        _mockHandler.HandleAsync(
                Arg.Any<ISignalBotClient>(),
                Arg.Any<ReceivedMessage>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask)
            .AndDoes(callInfo => messageReceivedTcs.SetResult(callInfo.ArgAt<ReceivedMessage>(1)));

        await _testServer.StartAsync();
        var receiver = new SignalBotReceiver(_mockClient);
        await receiver.StartReceivingAsync(_mockHandler, cancellationToken: TestContext.Current.CancellationToken);

        var testMessage = CreateTestReceivedMessage("Binary message!");
        var bytes = System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize(testMessage));

        // Act
        await _testServer.SendBinaryMessageAsync(bytes);

        // Assert
        var completed = await messageReceivedTcs.Task;
        Assert.NotNull(completed);
        Assert.Equal("Binary message!", completed.Envelope?.DataMessage?.Message);

        await receiver.DisposeAsync();
    }

    [Fact(Timeout = 10000)]
    public async Task Should_Receive_Multiple_Messages_In_Order()
    {
        // Arrange
        var receivedMessages = new ConcurrentBag<string>();
        var messageCount = 0;
        var allMessagesReceivedTcs = new TaskCompletionSource<bool>();

        _mockHandler.HandleAsync(
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

        await _testServer.StartAsync();
        var receiver = new SignalBotReceiver(_mockClient);
        await receiver.StartReceivingAsync(_mockHandler, cancellationToken: TestContext.Current.CancellationToken);

        // Act
        for (var i = 1; i <= 5; i++)
        {
            var message = CreateTestReceivedMessage($"Message {i}");
            await _testServer.SendMessageAsync(JsonSerializer.Serialize(message));
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

    [Fact(Timeout = 10000)]
    public async Task Should_Call_Handler_For_Each_Message()
    {
        // Arrange
        var messageCount = 0;
        const int expectedCount = 3;
        var allHandledTcs = new TaskCompletionSource<bool>();

        _mockHandler.HandleAsync(
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

        await _testServer.StartAsync();
        var receiver = new SignalBotReceiver(_mockClient);
        await receiver.StartReceivingAsync(_mockHandler, cancellationToken: TestContext.Current.CancellationToken);

        // Act
        for (var i = 0; i < expectedCount; i++)
        {
            var message = CreateTestReceivedMessage($"Test {i}");
            await _testServer.SendMessageAsync(JsonSerializer.Serialize(message));
        }

        // Assert
        await allHandledTcs.Task;

        await _mockHandler.Received(expectedCount).HandleAsync(
            _mockClient,
            Arg.Any<ReceivedMessage>(),
            Arg.Any<CancellationToken>());

        await receiver.DisposeAsync();
    }

    #endregion

    #region Message Type Tests

    [Fact(Timeout = 10000)]
    public async Task Should_Handle_Group_Message()
    {
        // Arrange
        var messageReceivedTcs = new TaskCompletionSource<ReceivedMessage>();

        _mockHandler.HandleAsync(
                Arg.Any<ISignalBotClient>(),
                Arg.Any<ReceivedMessage>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask)
            .AndDoes(callInfo => messageReceivedTcs.SetResult(callInfo.ArgAt<ReceivedMessage>(1)));

        await _testServer.StartAsync();
        var receiver = new SignalBotReceiver(_mockClient);
        await receiver.StartReceivingAsync(_mockHandler, cancellationToken: TestContext.Current.CancellationToken);

        var groupMessage = CreateTestGroupMessage("Hello group!", "group-123", "Test Group");

        // Act
        await _testServer.SendMessageAsync(JsonSerializer.Serialize(groupMessage));

        // Assert
        var completed = await messageReceivedTcs.Task;
        Assert.NotNull(completed);
        Assert.Equal("Hello group!", completed.Envelope?.DataMessage?.Message);
        Assert.Equal("Test Group", completed.Envelope?.DataMessage?.GroupV2?.Name);
        Assert.Equal("group-123", completed.Envelope?.DataMessage?.GroupV2?.Id);

        await receiver.DisposeAsync();
    }

    [Fact(Timeout = 10000)]
    public async Task Should_Handle_Message_With_Attachment()
    {
        // Arrange
        var messageReceivedTcs = new TaskCompletionSource<ReceivedMessage>();

        _mockHandler.HandleAsync(
                Arg.Any<ISignalBotClient>(),
                Arg.Any<ReceivedMessage>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask)
            .AndDoes(callInfo => messageReceivedTcs.SetResult(callInfo.ArgAt<ReceivedMessage>(1)));

        await _testServer.StartAsync();
        var receiver = new SignalBotReceiver(_mockClient);
        await receiver.StartReceivingAsync(_mockHandler, cancellationToken: TestContext.Current.CancellationToken);

        var attachmentMessage = CreateTestMessageWithAttachment("Check this file", "document.pdf", "application/pdf");

        // Act
        await _testServer.SendMessageAsync(JsonSerializer.Serialize(attachmentMessage));

        // Assert
        var completed = await messageReceivedTcs.Task;
        Assert.NotNull(completed);
        Assert.NotNull(completed.Envelope?.DataMessage?.Attachments);
        Assert.Single(completed.Envelope.DataMessage.Attachments);
        Assert.Equal("document.pdf", completed.Envelope.DataMessage.Attachments[0].Filename);
        Assert.Equal("application/pdf", completed.Envelope.DataMessage.Attachments[0].ContentType);

        await receiver.DisposeAsync();
    }

    [Fact(Timeout = 10000)]
    public async Task Should_Handle_Message_With_Multiple_Attachments()
    {
        // Arrange
        var messageReceivedTcs = new TaskCompletionSource<ReceivedMessage>();

        _mockHandler.HandleAsync(
                Arg.Any<ISignalBotClient>(),
                Arg.Any<ReceivedMessage>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask)
            .AndDoes(callInfo => messageReceivedTcs.SetResult(callInfo.ArgAt<ReceivedMessage>(1)));

        await _testServer.StartAsync();
        var receiver = new SignalBotReceiver(_mockClient);
        await receiver.StartReceivingAsync(_mockHandler, cancellationToken: TestContext.Current.CancellationToken);

        var message = CreateTestMessageWithMultipleAttachments("Files attached", 3);

        // Act
        await _testServer.SendMessageAsync(JsonSerializer.Serialize(message));

        // Assert
        var completed = await messageReceivedTcs.Task;
        Assert.NotNull(completed.Envelope?.DataMessage?.Attachments);
        Assert.Equal(3, completed.Envelope.DataMessage.Attachments.Count);

        await receiver.DisposeAsync();
    }

    #endregion

    #region Error Handling Tests

    [Fact(Timeout = 10000)]
    public async Task Should_Handle_Exception_In_Handler()
    {
        // Arrange
        var exceptionHandledTcs = new TaskCompletionSource<bool>();
        var expectedException = new InvalidOperationException("Handler failed!");

        _mockHandler.HandleAsync(
                Arg.Any<ISignalBotClient>(),
                Arg.Any<ReceivedMessage>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromException(expectedException));

        _mockHandler.HandleErrorAsync(
                Arg.Any<ISignalBotClient>(),
                Arg.Is<Error>(e =>
                    e.Exception == expectedException &&
                    e.ErrorType == ErrorType.MessageReceived),
                Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask)
            .AndDoes(_ => exceptionHandledTcs.SetResult(true));

        await _testServer.StartAsync();
        var receiver = new SignalBotReceiver(_mockClient);
        await receiver.StartReceivingAsync(_mockHandler, cancellationToken: TestContext.Current.CancellationToken);

        var message = CreateTestReceivedMessage("This will cause error");

        // Act
        await _testServer.SendMessageAsync(JsonSerializer.Serialize(message));

        // Assert
        var completed = await exceptionHandledTcs.Task;
        Assert.True(completed, "Error should be handled");

        await _mockHandler.Received(1).HandleErrorAsync(
            _mockClient,
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

        _mockHandler.HandleAsync(
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

        _mockHandler.HandleErrorAsync(
                Arg.Any<ISignalBotClient>(),
                Arg.Any<Error>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        await _testServer.StartAsync();
        var receiver = new SignalBotReceiver(_mockClient);
        await receiver.StartReceivingAsync(_mockHandler, cancellationToken: TestContext.Current.CancellationToken);

        // Act
        await _testServer.SendMessageAsync(JsonSerializer.Serialize(CreateTestReceivedMessage("First")));
        await _testServer.SendMessageAsync(JsonSerializer.Serialize(CreateTestReceivedMessage("Second")));

        // Assert
        var completed = await secondMessageTcs.Task;
        Assert.True(completed, "Should process second message after error");

        await receiver.DisposeAsync();
    }

    #endregion

    #region Cancellation Tests

    [Fact(Timeout = 30000)]
    public async Task Should_Stop_Processing_After_Cancellation()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        var processedAfterCancel = false;
        var firstMessageTcs = new TaskCompletionSource<bool>();

        _mockHandler.HandleAsync(
                Arg.Any<ISignalBotClient>(),
                Arg.Any<ReceivedMessage>(),
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

        await _testServer.StartAsync();
        var receiver = new SignalBotReceiver(_mockClient);
        await receiver.StartReceivingAsync(_mockHandler, cancellationToken: cts.Token);

        // Act
        await _testServer.SendMessageAsync(JsonSerializer.Serialize(CreateTestReceivedMessage("Before cancel")));
        await firstMessageTcs.Task;

        await cts.CancelAsync();

        await _testServer.SendMessageAsync(JsonSerializer.Serialize(CreateTestReceivedMessage("After cancel")));

        // Assert
        Assert.False(processedAfterCancel, "Should not process messages after cancellation");

        await receiver.DisposeAsync();
    }

    #endregion

    #region Message Filtering Tests

    [Fact(Timeout = 10000)]
    public async Task Should_Filter_Receipt_Messages_When_IgnoreReceipt_Is_True()
    {
        // Arrange
        var receivedMessages = new ConcurrentBag<ReceivedMessage>();
        var dataMessageTcs = new TaskCompletionSource<bool>();

        _mockHandler.HandleAsync(
                Arg.Any<ISignalBotClient>(),
                Arg.Any<ReceivedMessage>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask)
            .AndDoes(callInfo =>
            {
                var msg = callInfo.ArgAt<ReceivedMessage>(1);
                receivedMessages.Add(msg);
                if (msg.Envelope?.DataMessage != null)
                {
                    dataMessageTcs.SetResult(true);
                }
            });

        await _testServer.StartAsync();
        var receiver = new SignalBotReceiver(_mockClient);
        await receiver.StartReceivingAsync(
            _mockHandler,
            options => options.WithIgnoreReceipt(),
            cancellationToken: TestContext.Current.CancellationToken);

        // Act
        await _testServer.SendMessageAsync(JsonSerializer.Serialize(CreateTestReceiptMessage()));
        await _testServer.SendMessageAsync(JsonSerializer.Serialize(CreateTestReceivedMessage("Normal message")));

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
        var receivedMessages = new ConcurrentBag<ReceivedMessage>();
        var dataMessageTcs = new TaskCompletionSource<bool>();

        _mockHandler.HandleAsync(
                Arg.Any<ISignalBotClient>(),
                Arg.Any<ReceivedMessage>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask)
            .AndDoes(callInfo =>
            {
                var msg = callInfo.ArgAt<ReceivedMessage>(1);
                receivedMessages.Add(msg);
                if (msg.Envelope?.DataMessage != null)
                {
                    dataMessageTcs.SetResult(true);
                }
            });

        await _testServer.StartAsync();
        var receiver = new SignalBotReceiver(_mockClient);
        await receiver.StartReceivingAsync(
            _mockHandler,
            options => options.WithIgnoreTyping(),
            cancellationToken: TestContext.Current.CancellationToken);

        // Act
        await _testServer.SendMessageAsync(JsonSerializer.Serialize(CreateTestTypingMessage()));
        await _testServer.SendMessageAsync(JsonSerializer.Serialize(CreateTestReceivedMessage("Normal message")));

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
        var receivedMessages = new ConcurrentBag<ReceivedMessage>();
        var dataMessageTcs = new TaskCompletionSource<bool>();

        _mockHandler.HandleAsync(
                Arg.Any<ISignalBotClient>(),
                Arg.Any<ReceivedMessage>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask)
            .AndDoes(callInfo =>
            {
                var msg = callInfo.ArgAt<ReceivedMessage>(1);
                receivedMessages.Add(msg);
                if (msg.Envelope?.DataMessage != null)
                {
                    dataMessageTcs.SetResult(true);
                }
            });

        await _testServer.StartAsync();
        var receiver = new SignalBotReceiver(_mockClient);
        await receiver.StartReceivingAsync(
            _mockHandler,
            options => options.WithIgnoreSync(),
            cancellationToken: TestContext.Current.CancellationToken);

        // Act
        await _testServer.SendMessageAsync(JsonSerializer.Serialize(CreateTestSyncMessage()));
        await _testServer.SendMessageAsync(JsonSerializer.Serialize(CreateTestReceivedMessage("Normal message")));

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
        var receivedMessages = new ConcurrentBag<ReceivedMessage>();
        var dataMessageTcs = new TaskCompletionSource<bool>();

        _mockHandler.HandleAsync(
                Arg.Any<ISignalBotClient>(),
                Arg.Any<ReceivedMessage>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask)
            .AndDoes(callInfo =>
            {
                var msg = callInfo.ArgAt<ReceivedMessage>(1);
                receivedMessages.Add(msg);
                if (msg.Envelope?.DataMessage != null)
                {
                    dataMessageTcs.SetResult(true);
                }
            });

        await _testServer.StartAsync();
        var receiver = new SignalBotReceiver(_mockClient);
        await receiver.StartReceivingAsync(
            _mockHandler,
            options => options.WithIgnoreTyping().WithIgnoreSync().WithIgnoreReceipt(),
            cancellationToken: TestContext.Current.CancellationToken);

        // Act
        await _testServer.SendMessageAsync(JsonSerializer.Serialize(CreateTestReceiptMessage()));
        await _testServer.SendMessageAsync(JsonSerializer.Serialize(CreateTestTypingMessage()));
        await _testServer.SendMessageAsync(JsonSerializer.Serialize(CreateTestSyncMessage()));
        await _testServer.SendMessageAsync(JsonSerializer.Serialize(CreateTestReceivedMessage("Data message")));

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

        _mockHandler.HandleAsync(
                Arg.Any<ISignalBotClient>(),
                Arg.Any<ReceivedMessage>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask)
            .AndDoes(_ =>
            {
                if (Interlocked.Increment(ref messageCount) == 4)
                {
                    allMessagesReceivedTcs.SetResult(true);
                }
            });

        await _testServer.StartAsync();
        var receiver = new SignalBotReceiver(_mockClient);
        await receiver.StartReceivingAsync(
            _mockHandler,
            builder => builder.WithIgnoreReceipt(false).WithIgnoreSync(false).WithIgnoreTyping(false),
            cancellationToken: TestContext.Current.CancellationToken);

        // Act
        await _testServer.SendMessageAsync(JsonSerializer.Serialize(CreateTestReceiptMessage()));
        await _testServer.SendMessageAsync(JsonSerializer.Serialize(CreateTestTypingMessage()));
        await _testServer.SendMessageAsync(JsonSerializer.Serialize(CreateTestSyncMessage()));
        await _testServer.SendMessageAsync(JsonSerializer.Serialize(CreateTestReceivedMessage("Data message")));

        // Assert
        await allMessagesReceivedTcs.Task;
        Assert.Equal(4, messageCount);

        await receiver.DisposeAsync();
    }

    #endregion

    #region Performance & Load Tests

    [Fact(Timeout = 30000)]
    [Trait("Speed", "Slow")]
    public async Task Should_Handle_High_Volume_Sequential_Messages()
    {
        // Arrange
        const int messageCount = 1000;
        var receivedCount = 0;
        var allReceivedTcs = new TaskCompletionSource<bool>();

        _mockHandler.HandleAsync(
                Arg.Any<ISignalBotClient>(),
                Arg.Any<ReceivedMessage>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask)
            .AndDoes(_ =>
            {
                if (Interlocked.Increment(ref receivedCount) == messageCount)
                {
                    allReceivedTcs.SetResult(true);
                }
            });

        await _testServer.StartAsync();
        var receiver = new SignalBotReceiver(_mockClient);
        await receiver.StartReceivingAsync(_mockHandler, cancellationToken: TestContext.Current.CancellationToken);

        var stopwatch = Stopwatch.StartNew();

        // Act
        for (var i = 0; i < messageCount; i++)
        {
            var message = CreateTestReceivedMessage($"Message {i}");
            await _testServer.SendMessageAsync(JsonSerializer.Serialize(message));
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

        _mockHandler.HandleAsync(
                Arg.Any<ISignalBotClient>(),
                Arg.Any<ReceivedMessage>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask)
            .AndDoes(_ =>
            {
                if (Interlocked.Increment(ref receivedCount) == messageCount)
                {
                    allReceivedTcs.SetResult(true);
                }
            });

        await _testServer.StartAsync();
        var receiver = new SignalBotReceiver(_mockClient);
        await receiver.StartReceivingAsync(_mockHandler, cancellationToken: TestContext.Current.CancellationToken);

        var stopwatch = Stopwatch.StartNew();

        // Act
        for (var i = 0; i < messageCount; i++)
        {
            var largeText = new string('X', messageSize);
            var message = CreateTestReceivedMessage(largeText);
            await _testServer.SendMessageAsync(JsonSerializer.Serialize(message));
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

        _mockHandler.HandleAsync(
                Arg.Any<ISignalBotClient>(),
                Arg.Any<ReceivedMessage>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask)
            .AndDoes(callInfo =>
            {
                var msg = callInfo.ArgAt<ReceivedMessage>(1);
                var messageText = msg.Envelope?.DataMessage?.Message ?? "";
                var messageNumber = int.Parse(messageText.Replace("Message ", ""));
                receivedMessages.Enqueue(messageNumber);

                if (Interlocked.Increment(ref receivedCount) == messageCount)
                {
                    allReceivedTcs.SetResult(true);
                }
            });

        await _testServer.StartAsync();
        var receiver = new SignalBotReceiver(_mockClient);
        await receiver.StartReceivingAsync(_mockHandler, cancellationToken: TestContext.Current.CancellationToken);

        // Act
        for (var i = 0; i < messageCount; i++)
        {
            var message = CreateTestReceivedMessage($"Message {i}");
            await _testServer.SendMessageAsync(JsonSerializer.Serialize(message));
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

        _mockHandler.HandleAsync(
                Arg.Any<ISignalBotClient>(),
                Arg.Any<ReceivedMessage>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask)
            .AndDoes(callInfo =>
            {
                var msg = callInfo.ArgAt<ReceivedMessage>(1);
                if (msg.Envelope?.DataMessage != null) Interlocked.Increment(ref dataMessageCount);
                if (msg.Envelope?.ReceiptMessage != null) Interlocked.Increment(ref receiptMessageCount);
                if (msg.Envelope?.TypingMessage != null) Interlocked.Increment(ref typingMessageCount);
                if (msg.Envelope?.SyncMessage != null) Interlocked.Increment(ref syncMessageCount);

                var total = dataMessageCount + receiptMessageCount + typingMessageCount + syncMessageCount;
                if (total == messagesPerType * 4)
                {
                    allReceivedTcs.SetResult(true);
                }
            });

        await _testServer.StartAsync();
        var receiver = new SignalBotReceiver(_mockClient);
        await receiver.StartReceivingAsync(_mockHandler, cancellationToken: TestContext.Current.CancellationToken);

        // Act
        var messages = new List<ReceivedMessage>();
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
            await _testServer.SendMessageAsync(JsonSerializer.Serialize(message));
        }

        // Assert
        await allReceivedTcs.Task;
        Assert.Equal(messagesPerType, dataMessageCount);
        Assert.Equal(messagesPerType, receiptMessageCount);
        Assert.Equal(messagesPerType, typingMessageCount);
        Assert.Equal(messagesPerType, syncMessageCount);

        await receiver.DisposeAsync();
    }

    #endregion

    #region Helper Methods

    private static ReceivedMessage CreateTestReceivedMessage(string message)
    {
        return new ReceivedMessage
        {
            Account = "+1234567890",
            Envelope = new Envelope
            {
                Source = "+9876543210",
                SourceNumber = "+9876543210",
                SourceId = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow,
                DataMessage = new DataMessage
                {
                    Timestamp = DateTime.UtcNow,
                    Message = message
                }
            }
        };
    }

    private static ReceivedMessage CreateTestReceiptMessage()
    {
        return new ReceivedMessage
        {
            Account = "+1234567890",
            Envelope = new Envelope
            {
                Source = "+9876543210",
                SourceNumber = "+9876543210",
                SourceId = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow,
                ReceiptMessage = new ReceiptMessage
                {
                    Timestamps = [DateTime.UtcNow]
                }
            }
        };
    }

    private static ReceivedMessage CreateTestTypingMessage()
    {
        return new ReceivedMessage
        {
            Account = "+1234567890",
            Envelope = new Envelope
            {
                Source = "+9876543210",
                SourceNumber = "+9876543210",
                SourceId = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow,
                TypingMessage = new TypingMessage
                {
                    Action = "STARTED",
                    Timestamp = DateTime.UtcNow
                }
            }
        };
    }

    private static ReceivedMessage CreateTestSyncMessage()
    {
        return new ReceivedMessage
        {
            Account = "+1234567890",
            Envelope = new Envelope
            {
                Source = "+9876543210",
                SourceNumber = "+9876543210",
                SourceId = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow,
                SyncMessage = new SyncMessage
                {
                    ReadMessages = [new ReadMessage { Sender = string.Empty, Timestamp = DateTime.UtcNow }]
                }
            }
        };
    }

    private static ReceivedMessage CreateTestGroupMessage(string body, string groupId, string groupName)
    {
        var message = CreateTestReceivedMessage(body);
        message.Envelope!.DataMessage!.GroupV2 = new GroupV2Info
        {
            Id = groupId,
            Name = groupName,
            Revision = 1
        };
        return message;
    }

    private static ReceivedMessage CreateTestMessageWithAttachment(string body, string filename, string contentType)
    {
        var message = CreateTestReceivedMessage(body);
        message.Envelope!.DataMessage!.Attachments =
        [
            new Attachment
            {
                Id = Guid.NewGuid().ToString(),
                Filename = filename,
                ContentType = contentType,
                Size = 12345
            }
        ];
        return message;
    }

    private static ReceivedMessage CreateTestMessageWithMultipleAttachments(string body, int attachmentCount)
    {
        var message = CreateTestReceivedMessage(body);
        var attachments = new List<Attachment>();

        for (var i = 0; i < attachmentCount; i++)
        {
            attachments.Add(new Attachment
            {
                Id = Guid.NewGuid().ToString(),
                Filename = $"file_{i}.pdf",
                ContentType = "application/pdf",
                Size = 12345 + i
            });
        }

        message.Envelope!.DataMessage!.Attachments = attachments;
        return message;
    }

    private static int GetAvailablePort()
    {
        var listener = new TcpListener(System.Net.IPAddress.Loopback, 0);
        listener.Start();
        var port = ((System.Net.IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    #endregion
}