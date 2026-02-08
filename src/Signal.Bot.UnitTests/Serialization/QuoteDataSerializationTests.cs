using System.Text.Json;
using Signal.Bot.Serialization;
using Signal.Bot.Types;

namespace Signal.Bot.UnitTests.Serialization;

public class QuoteDataSerializationTests
{
    [Fact(Timeout = 5000)]
    public void TestQuoteDataSerializationAndDeserialization()
    {
        // Arrange
        var quote = new QuoteData
        {
            Id = Guid.NewGuid(),
            Author = "+49123456789",
            Text = "Hello",
            Timestamp = new DateTime(2023, 10, 27, 10, 0, 0, DateTimeKind.Utc)
        };

        // Act
        var json = JsonSerializer.Serialize(quote, JsonBotSerializerContext.Default.QuoteData);
        var deserialized = JsonSerializer.Deserialize(json, JsonBotSerializerContext.Default.QuoteData);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal(quote.Id, deserialized.Id);
        Assert.Equal(quote.Author, deserialized.Author);
        Assert.Equal(quote.Text, deserialized.Text);
        Assert.Equal(quote.Timestamp, deserialized.Timestamp);
    }

    [Fact(Timeout = 5000)]
    public void TestQuoteDataNullSerialization()
    {
        // Arrange
        var quote = new QuoteData();

        // Act
        var json = JsonSerializer.Serialize(quote, JsonBotSerializerContext.Default.QuoteData);
        var deserialized = JsonSerializer.Deserialize(json, JsonBotSerializerContext.Default.QuoteData);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal(Guid.Empty, deserialized.Id);
        Assert.Null(deserialized.Author);
        Assert.Null(deserialized.Text);
        Assert.Equal(default, deserialized.Timestamp);
    }
}
