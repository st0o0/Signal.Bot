using System.Text.Json;
using Signal.Bot.Serialization;
using Signal.Bot.Types;

namespace Signal.Bot.UnitTests.Serialization;

public class MentionSerializationTests
{
    [Fact(Timeout = 5000)]
    public void TestMentionSerializationAndDeserialization()
    {
        // Arrange
        var mention = new Mention
        {
            Start = 0,
            Length = 10,
            Id = Guid.NewGuid()
        };

        // Act
        var json = JsonSerializer.Serialize(mention, JsonBotSerializerContext.Default.Mention);
        var deserialized = JsonSerializer.Deserialize(json, JsonBotSerializerContext.Default.Mention);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal(mention.Start, deserialized.Start);
        Assert.Equal(mention.Length, deserialized.Length);
        Assert.Equal(mention.Id, deserialized.Id);
    }

    [Fact(Timeout = 5000)]
    public void TestMentionNullSerialization()
    {
        // Arrange
        var mention = new Mention();

        // Act
        var json = JsonSerializer.Serialize(mention, JsonBotSerializerContext.Default.Mention);
        var deserialized = JsonSerializer.Deserialize(json, JsonBotSerializerContext.Default.Mention);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Null(deserialized.Start);
        Assert.Null(deserialized.Length);
        Assert.Equal(Guid.Empty, deserialized.Id);
    }
}
