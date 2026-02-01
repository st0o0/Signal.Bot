using System.Net.Sockets;
using System.Text.Json;
using NSubstitute;
using Signal.Bot.IntegrationTests.Internal;
using Signal.Bot.Polling;
using Signal.Bot.Types;

namespace Signal.Bot.IntegrationTests;

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

        // Setup client
        _mockClient.BaseUrl.Returns($"localhost:{serverPort}");
        _mockClient.Number.Returns("+1234567890");
        _mockClient.JsonSerializerOptions.Returns(new JsonSerializerOptions());
    }

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
        var json = JsonSerializer.Serialize(testMessage);

        // Act
        await _testServer.SendMessageAsync(json);

        // Assert
        var completed = await messageReceivedTcs.Task;

        Assert.NotNull(completed);
        Assert.Equal("Hello from server!", completed.Envelope?.DataMessage?.Body);

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
        var json = JsonSerializer.Serialize(testMessage);
        var bytes = System.Text.Encoding.UTF8.GetBytes(json);

        // Act
        await _testServer.SendBinaryMessageAsync(bytes);

        // Assert
        var completed = await messageReceivedTcs.Task;

        Assert.NotNull(completed);
        Assert.Equal("Binary message!", completed.Envelope?.DataMessage?.Body);

        await receiver.DisposeAsync();
    }

    [Fact(Timeout = 10000)]
    public async Task Should_Receive_Multiple_Messages_In_Order()
    {
        // Arrange
        var receivedMessages = new System.Collections.Concurrent.ConcurrentBag<string>();
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
                receivedMessages.Add(msg.Envelope?.DataMessage?.Body ?? "");

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
            var json = JsonSerializer.Serialize(message);
            await _testServer.SendMessageAsync(json);
        }

        // Assert
        var completed = await allMessagesReceivedTcs.Task;

        Assert.True(completed, "Should receive all messages");
        Assert.Equal(5, receivedMessages.Count);

        for (var i = 1; i <= 5; i++)
        {
            Assert.Contains($"Message {i}", receivedMessages);
        }

        await receiver.DisposeAsync();
    }

    [Fact(Timeout = 10000)]
    public async Task Should_Handle_Server_Disconnect()
    {
        // Arrange
        var disconnectTcs = new TaskCompletionSource<bool>();

        _mockHandler.HandleErrorAsync(
                Arg.Any<ISignalBotClient>(),
                Arg.Is<Error>(e => e.Source == ErrorSource.DisconnectionHappened),
                Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask)
            .AndDoes(_ => disconnectTcs.SetResult(true));

        await _testServer.StartAsync();

        var receiver = new SignalBotReceiver(_mockClient);
        await receiver.StartReceivingAsync(_mockHandler, cancellationToken: TestContext.Current.CancellationToken);

        // Act
        await _testServer.DisconnectAsync();

        // Assert
        var completed = await disconnectTcs.Task;

        Assert.True(completed, "Should handle disconnect");

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
            var json = JsonSerializer.Serialize(message);
            await _testServer.SendMessageAsync(json);
        }

        // Assert
        var completed = await allHandledTcs.Task;

        Assert.True(completed);

        await _mockHandler.Received(expectedCount).HandleAsync(
            _mockClient,
            Arg.Any<ReceivedMessage>(),
            Arg.Any<CancellationToken>());

        await receiver.DisposeAsync();
    }

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
                    e.Source == ErrorSource.MessageReceived),
                Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask)
            .AndDoes(_ => exceptionHandledTcs.SetResult(true));

        await _testServer.StartAsync();

        var receiver = new SignalBotReceiver(_mockClient);
        await receiver.StartReceivingAsync(_mockHandler, cancellationToken: TestContext.Current.CancellationToken);

        var message = CreateTestReceivedMessage("This will cause error");
        var json = JsonSerializer.Serialize(message);

        // Act
        await _testServer.SendMessageAsync(json);

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
                switch (count)
                {
                    case 1:
                        throw new Exception("First message fails");
                    case 2:
                        secondMessageTcs.SetResult(true);
                        break;
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

    [Fact]
    public async Task Should_Dispose_Cleanly()
    {
        // Arrange
        await _testServer.StartAsync();

        var receiver = new SignalBotReceiver(_mockClient);
        await receiver.StartReceivingAsync(_mockHandler, cancellationToken: TestContext.Current.CancellationToken);


        // Act
        await receiver.DisposeAsync();

        // Assert
        Assert.True(true, "Dispose completed without exception");
    }

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
        var json = JsonSerializer.Serialize(groupMessage);

        // Act
        await _testServer.SendMessageAsync(json);

        // Assert
        var completed = await messageReceivedTcs.Task;

        Assert.NotNull(completed);
        Assert.Equal("Hello group!", completed.Envelope?.DataMessage?.Body);
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
        var json = JsonSerializer.Serialize(attachmentMessage);

        // Act
        await _testServer.SendMessageAsync(json);

        // Assert
        var completed = await messageReceivedTcs.Task;

        Assert.NotNull(completed);
        Assert.NotNull(completed.Envelope?.DataMessage?.Attachments);
        Assert.Single(completed.Envelope.DataMessage.Attachments);
        Assert.Equal("document.pdf", completed.Envelope.DataMessage.Attachments[0].Filename);
        Assert.Equal("application/pdf", completed.Envelope.DataMessage.Attachments[0].ContentType);

        await receiver.DisposeAsync();
    }

    private static ReceivedMessage CreateTestReceivedMessage(string body)
    {
        return new ReceivedMessage
        {
            Account = "+1234567890",
            Subscription = 0,
            Envelope = new Envelope
            {
                Source = "+9876543210",
                SourceNumber = "+9876543210",
                SourceId = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow,
                DataMessage = new DataMessage
                {
                    Timestamp = DateTime.UtcNow,
                    Body = body
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

    private static int GetAvailablePort()
    {
        var listener = new TcpListener(System.Net.IPAddress.Loopback, 0);
        listener.Start();
        var port = ((System.Net.IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    public async ValueTask DisposeAsync()
    {
        await _testServer.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}