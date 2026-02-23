using System.Text.Json;
using Signal.Bot.Serialization;
using Signal.Bot.Types;

namespace Signal.Bot.UnitTests.Serialization;

public class QuoteDataSerializationTests
{
    [Fact(Timeout = 5000)]
    public void TestQuoteSerializationAndDeserialization()
    {
        // Arrange
        var timestamp = 1707523200000;
        var quote = new Quote
        {
            Id = DateTimeOffset.FromUnixTimeMilliseconds(1770354634885).UtcDateTime,
            Author = "+49123456789",
            Text = "Hello",
        };

        // Act
        var json = JsonSerializer.Serialize(quote, JsonBotSerializerContext.Default.Quote);
        var deserialized = JsonSerializer.Deserialize(json, JsonBotSerializerContext.Default.Quote);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal(quote.Id, deserialized.Id);
        Assert.Equal(quote.Author, deserialized.Author);
        Assert.Equal(quote.Text, deserialized.Text);
    }

    [Fact(Timeout = 5000)]
    public void TestQuoteNullSerialization()
    {
        // Arrange
        var quote = new Quote();

        // Act
        var json = JsonSerializer.Serialize(quote, JsonBotSerializerContext.Default.Quote);
        var deserialized = JsonSerializer.Deserialize(json, JsonBotSerializerContext.Default.Quote);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Null(deserialized.Id);
        Assert.Null(deserialized.Author);
        Assert.Null(deserialized.Text);
    }

    [Fact(Timeout = 5000)]
    public void TestRealQuoteDeserialization()
    {
        const string json = """
                            {
                              "id": 1770354634885,
                              "author": "Leberkas Ipsum",
                              "authorNumber": "Amet Redacted",
                              "authorUuid": "8bfa5b02-3675-4c27-97b6-cf539f1bf2e2",
                              "text": "Ketchup Anonym",
                              "mentions": [
                                {
                                  "name": "Leberkas Anonym",
                                  "number": "Leberkas Amet",
                                  "uuid": "445645b2-6902-435d-8086-c3b934de27d0",
                                  "start": 80509,
                                  "length": 37640
                                }
                              ],
                              "attachments": []
                            }
                            """;
        var quote = JsonSerializer.Deserialize(json, JsonBotSerializerContext.Default.Quote);
        Assert.NotNull(quote);
        Assert.Equal(DateTimeOffset.FromUnixTimeMilliseconds(1770354634885).DateTime, quote.Id);
        Assert.Equal("Leberkas Ipsum", quote.Author);
        Assert.Equal("Ketchup Anonym", quote.Text);
        Assert.NotNull(quote.Attachments);
        Assert.Empty(quote.Attachments);
    }
}