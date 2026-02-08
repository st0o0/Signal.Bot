using System.Text.Json;
using Signal.Bot.Serialization;
using Signal.Bot.Types;

namespace Signal.Bot.UnitTests.Serialization;

public class LoggingConfigurationSerializationTests
{
    [Fact(Timeout = 5000)]
    public void TestLoggingConfigurationSerializationAndDeserialization()
    {
        // Arrange
        var config = new LoggingConfiguration
        {
            Level = "debug"
        };

        // Act
        var json = JsonSerializer.Serialize(config, JsonBotSerializerContext.Default.LoggingConfiguration);
        var deserialized = JsonSerializer.Deserialize(json, JsonBotSerializerContext.Default.LoggingConfiguration);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal(config.Level, deserialized.Level);
    }

    [Fact(Timeout = 5000)]
    public void TestLoggingConfigurationNullSerialization()
    {
        // Arrange
        var config = new LoggingConfiguration();

        // Act
        var json = JsonSerializer.Serialize(config, JsonBotSerializerContext.Default.LoggingConfiguration);
        var deserialized = JsonSerializer.Deserialize(json, JsonBotSerializerContext.Default.LoggingConfiguration);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Null(deserialized.Level);
    }
}
