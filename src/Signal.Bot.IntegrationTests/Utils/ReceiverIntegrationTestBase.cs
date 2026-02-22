using System.Net.Sockets;
using System.Text.Json;
using NSubstitute;
using Signal.Bot.Types;

namespace Signal.Bot.IntegrationTests.Utils;

public abstract class ReceiverIntegrationTestBase : IAsyncDisposable
{
    protected readonly WebSocketTestServer TestServer;
    protected readonly ISignalBotClient MockClient;
    protected readonly IReceivedMessageHandler MockHandler;

    protected ReceiverIntegrationTestBase()
    {
        var serverPort = GetAvailablePort();
        TestServer = new WebSocketTestServer(serverPort);

        MockClient = Substitute.For<ISignalBotClient>();
        MockHandler = Substitute.For<IReceivedMessageHandler>();

        MockClient.BaseUrl.Returns($"localhost:{serverPort}");
        MockClient.Number.Returns("+1234567890");
        MockClient.JsonSerializerOptions.Returns(new JsonSerializerOptions());
    }

    public async ValueTask DisposeAsync()
    {
        await TestServer.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    protected static ReceivedMessage CreateTestReceivedMessage(string message)
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
                    Message = message,
                    Timestamp = DateTime.UtcNow
                }
            }
        };
    }

    protected static ReceivedMessage CreateTestReceiptMessage()
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
                    Type = "DELIVERY",
                    Timestamps = [DateTime.UtcNow]
                }
            }
        };
    }

    protected static ReceivedMessage CreateTestTypingMessage()
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
                    Action = TypingAction.Started,
                    Timestamp = DateTime.UtcNow
                }
            }
        };
    }

    protected static ReceivedMessage CreateTestSyncMessage()
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

    protected static ReceivedMessage CreateTestGroupMessage(string body, string groupId, string groupName)
    {
        var message = CreateTestReceivedMessage(body);
        message.Envelope!.DataMessage!.GroupV2 = new GroupInfo
        {
            Id = groupId,
            Name = groupName,
            Revision = 1
        };
        return message;
    }

    protected static ReceivedMessage CreateTestMessageWithAttachment(string body, string filename, string contentType)
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

    protected static ReceivedMessage CreateTestMessageWithMultipleAttachments(string body, int attachmentCount)
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

    protected static int GetAvailablePort()
    {
        var listener = new TcpListener(System.Net.IPAddress.Loopback, 0);
        listener.Start();
        var port = ((System.Net.IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }
}