using System.Text.Json;
using Signal.Bot.Serialization;
using Signal.Bot.Types;

namespace Signal.Bot.UnitTests.Serialization;

public class PinMessageSerializationTests
{
    [Fact(Timeout = 5000)]
    public void TestReceivedPinMessageSerializationAndDeserialization()
    {
        // Arrange
        var pin = new ReceivedPinMessage
        {
            PinDuration = TimeSpan.FromSeconds(86400),
            TargetAuthor = "Carol",
            TargetAuthorNumber = "+4915177777777",
            TargetAuthorId = Guid.Parse("d4e5f6a7-b8c9-0123-defa-234567890123"),
            TargetSent = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        // Act
        var json = JsonSerializer.Serialize(pin, JsonBotSerializerContext.Default.ReceivedPinMessage);
        var deserialized = JsonSerializer.Deserialize(json, JsonBotSerializerContext.Default.ReceivedPinMessage);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal(TimeSpan.FromSeconds(86400), deserialized.PinDuration);
        Assert.Equal("Carol", deserialized.TargetAuthor);
        Assert.Equal(pin.TargetAuthorId, deserialized.TargetAuthorId);
        Assert.Equal(pin.TargetSent, deserialized.TargetSent);
    }

    [Fact(Timeout = 5000)]
    public void TestReceivedUnpinMessageSerializationAndDeserialization()
    {
        // Arrange
        var unpin = new ReceivedUnpinMessage
        {
            TargetAuthor = "Dave",
            TargetAuthorNumber = "+4915166666666",
            TargetAuthorId = Guid.Parse("e5f6a7b8-c9d0-1234-efab-345678901234"),
            TargetSent = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        // Act
        var json = JsonSerializer.Serialize(unpin, JsonBotSerializerContext.Default.ReceivedUnpinMessage);
        var deserialized = JsonSerializer.Deserialize(json, JsonBotSerializerContext.Default.ReceivedUnpinMessage);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal("Dave", deserialized.TargetAuthor);
        Assert.Equal(unpin.TargetAuthorId, deserialized.TargetAuthorId);
        Assert.Equal(unpin.TargetSent, deserialized.TargetSent);
    }

    [Fact(Timeout = 5000)]
    public void TestRealReceivedPinMessageDeserialization()
    {
        // Arrange
        const string json = """
                            {
                              "pinDurationSeconds": 3600,
                              "targetAuthor": "Grace",
                              "targetAuthorNumber": "+49123",
                              "targetAuthorUuid": "44444444-5555-6666-7777-888888888888",
                              "targetSentTimestamp": 1704067200000
                            }
                            """;

        // Act
        var result = JsonSerializer.Deserialize(json, JsonBotSerializerContext.Default.ReceivedPinMessage);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(TimeSpan.FromSeconds(3600), result.PinDuration);
        Assert.Equal("Grace", result.TargetAuthor);
        Assert.Equal(Guid.Parse("44444444-5555-6666-7777-888888888888"), result.TargetAuthorId);
        Assert.Equal(DateTime.UnixEpoch.AddMilliseconds(1704067200000), result.TargetSent);
    }
}
