using System.Text.Json;
using Signal.Bot.Serialization;
using Signal.Bot.Requests;
using Signal.Bot.Types;

namespace Signal.Bot.UnitTests.Serialization;

public class MessageSerializationTests
{
    [Fact(Timeout = 5000)]
    public void TestReceivedMessage_OptionalFieldsMissing_DeserializesCorrectly()
    {
        // Arrange
        var json = "{\"account\": \"msg123\", \"envelope\": {\"source\": \"src123\"}}";

        // Act
        var result = JsonSerializer.Deserialize<ReceivedMessage>(json, (JsonSerializerOptions)JsonBotAPI.Options);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("msg123", result.Account);
        Assert.NotNull(result.Envelope);
        Assert.Equal("src123", result.Envelope.Source);
        Assert.Null(result.Envelope.DataMessage);
        Assert.Null(result.Envelope.ReceiptMessage);
        Assert.Null(result.Envelope.TypingMessage);
    }

    [Fact(Timeout = 5000)]
    public void TestReceivedMessageSerializationAndDeserialization()
    {
        // Arrange
        var receivedMessage = new ReceivedMessage
        {
            Account = "msg123",
            Envelope = new Envelope
            {
                SourceId = Guid.Empty,
                SourceNumber = "msg123",
                Source = "msg123",
                DataMessage = new DataMessage
                {
                    Message = "Hello, World!"
                }
            }
        };

        // Act
        var json = JsonSerializer.Serialize(receivedMessage);
        var deserializedReceivedMessage = JsonSerializer.Deserialize<ReceivedMessage>(json);

        // Assert
        Assert.NotNull(deserializedReceivedMessage);
        Assert.NotNull(deserializedReceivedMessage.Envelope);
        Assert.NotNull(deserializedReceivedMessage.Envelope.DataMessage);
        Assert.Equal(receivedMessage.Account, deserializedReceivedMessage.Account);
        Assert.Equal(receivedMessage.Envelope.DataMessage.Message, deserializedReceivedMessage.Envelope.DataMessage.Message);
    }

    [Fact(Timeout = 5000)]
    public void TestRemoteDeleteMessageSerializationAndDeserialization()
    {
        // Arrange
        var remoteDeleteMessage = new Acknowledged
        {
            Timestamp = DateTime.Now
        };

        // Act
        var json = JsonSerializer.Serialize(remoteDeleteMessage);
        var deserializedRemoteDeleteMessage = JsonSerializer.Deserialize<Acknowledged>(json);

        // Assert
        Assert.NotNull(deserializedRemoteDeleteMessage);
        Assert.Equal(remoteDeleteMessage.Timestamp, deserializedRemoteDeleteMessage.Timestamp);
    }

    [Fact(Timeout = 5000)]
    public void TestSendMessageRequestSerializationAndDeserialization()
    {
        // Arrange
        var sendMessageRequest = new SendMessageRequest
        {
            Message = "recipientUuid123",
            QuoteAuthor = "Hello, World!"
        };

        // Act
        var json = JsonSerializer.Serialize(sendMessageRequest);
        var deserializedSendMessageRequest = JsonSerializer.Deserialize<SendMessageRequest>(json);

        // Assert
        Assert.NotNull(deserializedSendMessageRequest);
        Assert.Equal(sendMessageRequest.Message, deserializedSendMessageRequest.Message);
        Assert.Equal(sendMessageRequest.QuoteAuthor, deserializedSendMessageRequest.QuoteAuthor);
    }
}

