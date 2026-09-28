using System.Text.Json;
using Signal.Bot.Serialization;
using Signal.Bot.Types;

namespace Signal.Bot.UnitTests.Serialization;

public class AdminDeleteSerializationTests
{
    [Fact]
    public void TestAdminDeleteSerializationAndDeserialization()
    {
        // Arrange
        var adminDelete = new AdminDelete
        {
            TargetAuthor = "Bob",
            TargetAuthorNumber = "+4915188888888",
            TargetAuthorId = Guid.Parse("c3d4e5f6-a7b8-9012-cdef-123456789012"),
            TargetSent = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        // Act
        var json = JsonSerializer.Serialize(adminDelete, JsonBotSerializerContext.Default.AdminDelete);
        var deserialized = JsonSerializer.Deserialize(json, JsonBotSerializerContext.Default.AdminDelete);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal("Bob", deserialized.TargetAuthor);
        Assert.Equal("+4915188888888", deserialized.TargetAuthorNumber);
        Assert.Equal(adminDelete.TargetAuthorId, deserialized.TargetAuthorId);
        Assert.Equal(adminDelete.TargetSent, deserialized.TargetSent);
    }

    [Fact]
    public void TestAdminDeleteNullSerialization()
    {
        // Arrange
        var adminDelete = new AdminDelete();

        // Act
        var json = JsonSerializer.Serialize(adminDelete, JsonBotSerializerContext.Default.AdminDelete);
        var deserialized = JsonSerializer.Deserialize(json, JsonBotSerializerContext.Default.AdminDelete);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Null(deserialized.TargetAuthor);
        Assert.Null(deserialized.TargetAuthorNumber);
        Assert.Equal(default, deserialized.TargetSent);
    }
}
