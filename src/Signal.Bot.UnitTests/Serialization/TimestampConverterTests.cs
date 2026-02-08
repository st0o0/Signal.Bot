using System.Text.Json;
using Signal.Bot.Serialization;

namespace Signal.Bot.UnitTests.Serialization;

public class TimestampConverterTests
{
    private readonly JsonSerializerOptions _options = new()
    {
        Converters = { new TimestampConverter() }
    };

    [Fact(Timeout = 5000)]
    public void Read_ValidNumberTimestamp_ReturnsCorrectDateTime()
    {
        // Arrange
        const long milliseconds = 1707523200000; // 2024-02-10 00:00:00 UTC
        var json = milliseconds.ToString();

        // Act
        var result = JsonSerializer.Deserialize<DateTime>(json, _options);

        // Assert
        Assert.Equal(DateTimeOffset.FromUnixTimeMilliseconds(milliseconds).DateTime, result);
    }

    [Fact(Timeout = 5000)]
    public void Read_ValidStringTimestamp_ReturnsCorrectDateTime()
    {
        // Arrange
        const long milliseconds = 1707523200000;
        var json = $"\"{milliseconds}\"";

        // Act
        var result = JsonSerializer.Deserialize<DateTime>(json, _options);

        // Assert
        Assert.Equal(DateTimeOffset.FromUnixTimeMilliseconds(milliseconds).DateTime, result);
    }

    [Fact(Timeout = 5000)]
    public void Read_InvalidStringFormat_ThrowsJsonException()
    {
        // Arrange
        const string json = "\"not-a-number\"";

        // Act & Assert
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<DateTime>(json, _options));
    }

    [Fact(Timeout = 5000)]
    public void Read_UnsupportedTokenType_ThrowsJsonException()
    {
        // Arrange
        const string json = "true";

        // Act & Assert
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<DateTime>(json, _options));
    }

    [Fact(Timeout = 5000)]
    public void Write_DateTime_WritesStringTimestamp()
    {
        // Arrange
        var dateTime = new DateTime(2024, 2, 10, 0, 0, 0, DateTimeKind.Utc);
        var expectedMilliseconds = new DateTimeOffset(dateTime).ToUnixTimeMilliseconds().ToString();

        // Act
        var json = JsonSerializer.Serialize(dateTime, _options);

        // Assert
        Assert.Equal($"\"{expectedMilliseconds}\"", json);
    }
}