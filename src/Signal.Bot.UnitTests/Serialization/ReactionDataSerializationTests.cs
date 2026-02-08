using System.Text.Json;
using Signal.Bot.Serialization;
using Signal.Bot.Types;

namespace Signal.Bot.UnitTests.Serialization;

public class ReactionDataSerializationTests
{
    [Fact(Timeout = 5000)]
    public void TestReactionDataSerializationAndDeserialization()
    {
        // Arrange
        var reaction = new ReactionData
        {
            Emoji = "👍",
            Remove = false,
            TargetAuthor = "+49123456789",
            TargetSent = new DateTime(2023, 10, 27, 10, 0, 0, DateTimeKind.Utc)
        };

        // Act
        var json = JsonSerializer.Serialize(reaction, JsonBotSerializerContext.Default.ReactionData);
        var deserialized = JsonSerializer.Deserialize(json, JsonBotSerializerContext.Default.ReactionData);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal(reaction.Emoji, deserialized.Emoji);
        Assert.Equal(reaction.Remove, deserialized.Remove);
        Assert.Equal(reaction.TargetAuthor, deserialized.TargetAuthor);
        Assert.Equal(reaction.TargetSent, deserialized.TargetSent);
    }

    [Fact(Timeout = 5000)]
    public void TestReactionDataNullSerialization()
    {
        // Arrange
        var reaction = new ReactionData();

        // Act
        var json = JsonSerializer.Serialize(reaction, JsonBotSerializerContext.Default.ReactionData);
        var deserialized = JsonSerializer.Deserialize(json, JsonBotSerializerContext.Default.ReactionData);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Null(deserialized.Emoji);
        Assert.Null(deserialized.Remove);
        Assert.Null(deserialized.TargetAuthor);
        Assert.Equal(default, deserialized.TargetSent);
    }
}
