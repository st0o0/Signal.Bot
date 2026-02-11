using System.Text.Json;
using Signal.Bot.Requests;
using Signal.Bot.Types;

namespace Signal.Bot.UnitTests.Serialization;

public class SystemSerializationTests
{
    [Fact(Timeout = 5000)]
    public void TestErrorSerializationAndDeserialization()
    {
        // Arrange
        var error = new ErrorResponse
        {
            Message = "Not Found"
        };

        // Act
        var json = JsonSerializer.Serialize(error);
        var deserializedError = JsonSerializer.Deserialize<ErrorResponse>(json);

        // Assert
        Assert.NotNull(deserializedError);
        Assert.Equal(error.Message, deserializedError.Message);
    }

    [Fact(Timeout = 5000)]
    public void TestSetConfigurationRequestSerializationAndDeserialization()
    {
        // Arrange
        var setConfigurationRequest = new SetConfigurationRequest
        {
            Logging = new Logging { Level = "configKey123" }
        };

        // Act
        var json = JsonSerializer.Serialize(setConfigurationRequest);
        var deserializedSetConfigurationRequest = JsonSerializer.Deserialize<SetConfigurationRequest>(json);

        // Assert
        Assert.NotNull(deserializedSetConfigurationRequest);
        Assert.Equal(setConfigurationRequest.Logging.Level, deserializedSetConfigurationRequest.Logging!.Level);
    }

    [Fact(Timeout = 5000)]
    public void TestSetTypingIndicatorRequestSerializationAndDeserialization()
    {
        // Arrange
        var setTypingIndicatorRequest = new AddTypingIndicatorRequest("")
        {
            Recipient = "recipientUuid456",
        };

        // Act
        var json = JsonSerializer.Serialize(setTypingIndicatorRequest);
        var deserializedSetTypingIndicatorRequest = JsonSerializer.Deserialize<AddTypingIndicatorRequest>(json);

        // Assert
        Assert.NotNull(deserializedSetTypingIndicatorRequest);
        Assert.Equal(setTypingIndicatorRequest.Recipient, deserializedSetTypingIndicatorRequest.Recipient);
    }
}

