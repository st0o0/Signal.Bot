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

    [Fact(Timeout = 5000)]
    public void TestRealMentionDeserialization()
    {
        const string json = """
                            [
                              {
                                "name": "Ketchup Lorem",
                                "number": "Servus Amet",
                                "uuid": "dceab49f-60ec-447f-bba6-96127d776adc",
                                "start": 49552,
                                "length": 58220
                              }
                            ]
                            """;
        var mentions = JsonSerializer.Deserialize<List<Mention>>(json, JsonBotSerializerContext.Default.ListMention);
        Assert.NotNull(mentions);
        var mention = Assert.Single(mentions);
        Assert.Equal("Ketchup Lorem", mention.Name);
        Assert.Equal("Servus Amet", mention.Number);
        Assert.Equal(49552, mention.Start);
        Assert.Equal(58220, mention.Length);
    }
}