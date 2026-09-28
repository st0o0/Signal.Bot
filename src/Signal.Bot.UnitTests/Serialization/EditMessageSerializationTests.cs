using System.Text.Json;
using Signal.Bot.Serialization;
using Signal.Bot.Types;

namespace Signal.Bot.UnitTests.Serialization;

public class EditMessageSerializationTests
{
    [Fact]
    public void TestEditMessageSerializationAndDeserialization()
    {
        // Arrange
        var editMessage = new EditMessage
        {
            DataMessage = new DataMessage { Message = "Edited text" },
            TargetSent = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        // Act
        var json = JsonSerializer.Serialize(editMessage, JsonBotSerializerContext.Default.EditMessage);
        var deserialized = JsonSerializer.Deserialize(json, JsonBotSerializerContext.Default.EditMessage);

        // Assert
        Assert.NotNull(deserialized);
        Assert.NotNull(deserialized.DataMessage);
        Assert.Equal("Edited text", deserialized.DataMessage.Message);
        Assert.Equal(editMessage.TargetSent, deserialized.TargetSent);
    }

    [Fact]
    public void TestEditMessageNullSerialization()
    {
        // Arrange
        var editMessage = new EditMessage();

        // Act
        var json = JsonSerializer.Serialize(editMessage, JsonBotSerializerContext.Default.EditMessage);
        var deserialized = JsonSerializer.Deserialize(json, JsonBotSerializerContext.Default.EditMessage);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Null(deserialized.DataMessage);
        Assert.Equal(default, deserialized.TargetSent);
    }

    [Fact]
    public void TestRealEditMessageDeserialization()
    {
        // Arrange
        const string json = """
                            {
                              "dataMessage": {
                                "timestamp": 1704067200000,
                                "message": "Corrected text"
                              },
                              "targetSentTimestamp": 1704060000000
                            }
                            """;

        // Act
        var result = JsonSerializer.Deserialize(json, JsonBotSerializerContext.Default.EditMessage);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.DataMessage);
        Assert.Equal("Corrected text", result.DataMessage.Message);
        Assert.Equal(DateTime.UnixEpoch.AddMilliseconds(1704067200000), result.DataMessage.Timestamp);
        Assert.Equal(DateTime.UnixEpoch.AddMilliseconds(1704060000000), result.TargetSent);
    }
}
