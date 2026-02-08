using System.Text.Json;
using Signal.Bot.Serialization;
using Signal.Bot.Types;

namespace Signal.Bot.UnitTests.Serialization;

public class AboutSerializationTests
{
    [Fact(Timeout = 5000)]
    public void TestAboutSerializationAndDeserialization()
    {
        // Arrange
        var about = new About
        {
            Build = 123,
            Mode = "production",
            Version = "1.2.3",
            Versions = new List<string> { "v1", "v2" },
            Capabilities = new Dictionary<string, List<string>>
            {
                { "cap1", new List<string> { "a", "b" } }
            }
        };

        // Act
        var json = JsonSerializer.Serialize(about, JsonBotSerializerContext.Default.About);
        var deserialized = JsonSerializer.Deserialize(json, JsonBotSerializerContext.Default.About);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal(about.Build, deserialized.Build);
        Assert.Equal(about.Mode, deserialized.Mode);
        Assert.Equal(about.Version, deserialized.Version);
        Assert.Equal(about.Versions, deserialized.Versions);
        Assert.Equal(about.Capabilities, deserialized.Capabilities);
    }

    [Fact(Timeout = 5000)]
    public void TestAboutNullSerialization()
    {
        // Arrange
        var about = new About();

        // Act
        var json = JsonSerializer.Serialize(about, JsonBotSerializerContext.Default.About);
        var deserialized = JsonSerializer.Deserialize(json, JsonBotSerializerContext.Default.About);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Null(deserialized.Build);
        Assert.Null(deserialized.Capabilities);
        Assert.Null(deserialized.Mode);
        Assert.Null(deserialized.Version);
        Assert.Null(deserialized.Versions);
    }
}
