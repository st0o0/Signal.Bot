using System.Text.Json;
using Signal.Bot.Serialization;
using Signal.Bot.Types;

namespace Signal.Bot.UnitTests.Serialization;

public class StorySerializationTests
{
    [Fact]
    public void TestStoryMessageSerializationAndDeserialization()
    {
        // Arrange
        var story = new StoryMessage
        {
            AllowsReplies = true,
            GroupId = "group-abc-123",
            FileAttachment = new Attachment { ContentType = "image/png", Id = "attach-001", Size = 48000 }
        };

        // Act
        var json = JsonSerializer.Serialize(story, JsonBotSerializerContext.Default.StoryMessage);
        var deserialized = JsonSerializer.Deserialize(json, JsonBotSerializerContext.Default.StoryMessage);

        // Assert
        Assert.NotNull(deserialized);
        Assert.True(deserialized.AllowsReplies);
        Assert.Equal("group-abc-123", deserialized.GroupId);
        Assert.NotNull(deserialized.FileAttachment);
        Assert.Equal("image/png", deserialized.FileAttachment.ContentType);
        Assert.Null(deserialized.TextAttachment);
    }

    [Fact]
    public void TestStoryMessageWithTextAttachment()
    {
        // Arrange
        const string json = """
                            {
                              "allowsReplies": false,
                              "textAttachment": {
                                "text": "Hello Story",
                                "style": "bold",
                                "backgroundColor": "#FF0000",
                                "backgroundGradient": {
                                  "angle": 45,
                                  "startColor": "#FF0000",
                                  "endColor": "#0000FF",
                                  "colors": ["#FF0000", "#00FF00", "#0000FF"],
                                  "positions": [0.0, 0.5, 1.0]
                                }
                              }
                            }
                            """;

        // Act
        var result = JsonSerializer.Deserialize(json, JsonBotSerializerContext.Default.StoryMessage);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.AllowsReplies);
        Assert.NotNull(result.TextAttachment);
        Assert.Equal("Hello Story", result.TextAttachment.Text);
        Assert.Equal(TextStyleType.Bold, result.TextAttachment.Style);
        Assert.NotNull(result.TextAttachment.BackgroundGradient);
        Assert.Equal(45, result.TextAttachment.BackgroundGradient.Angle);
        Assert.Equal(3, result.TextAttachment.BackgroundGradient.Colors!.Count);
    }

    [Fact]
    public void TestStoryContextSerializationAndDeserialization()
    {
        // Arrange
        var context = new StoryContext
        {
            AuthorNumber = "+4915112345678",
            AuthorId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
            Sent = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        // Act
        var json = JsonSerializer.Serialize(context, JsonBotSerializerContext.Default.StoryContext);
        var deserialized = JsonSerializer.Deserialize(json, JsonBotSerializerContext.Default.StoryContext);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal("+4915112345678", deserialized.AuthorNumber);
        Assert.Equal(context.AuthorId, deserialized.AuthorId);
        Assert.Equal(context.Sent, deserialized.Sent);
    }

    [Fact]
    public void TestSyncStoryMessageSerializationAndDeserialization()
    {
        // Arrange
        var syncStory = new SyncStoryMessage
        {
            AllowsReplies = true,
            DestinationNumber = "+4915155555555",
            DestinationId = Guid.Parse("f6a7b8c9-d0e1-2345-abcd-456789012345"),
            GroupId = "group-xyz"
        };

        // Act
        var json = JsonSerializer.Serialize(syncStory, JsonBotSerializerContext.Default.SyncStoryMessage);
        var deserialized = JsonSerializer.Deserialize(json, JsonBotSerializerContext.Default.SyncStoryMessage);

        // Assert
        Assert.NotNull(deserialized);
        Assert.True(deserialized.AllowsReplies);
        Assert.Equal("+4915155555555", deserialized.DestinationNumber);
        Assert.Equal(syncStory.DestinationId, deserialized.DestinationId);
        Assert.Equal("group-xyz", deserialized.GroupId);
    }
}
