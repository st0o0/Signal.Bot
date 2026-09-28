using System.Text.Json;
using Signal.Bot.Serialization;
using Signal.Bot.Types;

namespace Signal.Bot.UnitTests.Serialization;

public class PollReceiveSerializationTests
{
    [Fact(Timeout = 5000)]
    public void TestPollVoteSerializationAndDeserialization()
    {
        // Arrange
        var vote = new PollVote
        {
            Author = "Alice",
            AuthorNumber = "+4915199999999",
            AuthorId = Guid.Parse("b2c3d4e5-f6a7-8901-bcde-f12345678901"),
            OptionIndexes = [0, 2],
            TargetSent = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            VoteCount = 5
        };

        // Act
        var json = JsonSerializer.Serialize(vote, JsonBotSerializerContext.Default.PollVote);
        var deserialized = JsonSerializer.Deserialize(json, JsonBotSerializerContext.Default.PollVote);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal("Alice", deserialized.Author);
        Assert.Equal(vote.AuthorId, deserialized.AuthorId);
        Assert.Equal([0, 2], deserialized.OptionIndexes);
        Assert.Equal(vote.TargetSent, deserialized.TargetSent);
        Assert.Equal(5, deserialized.VoteCount);
    }

    [Fact(Timeout = 5000)]
    public void TestPollVoteNullSerialization()
    {
        // Arrange
        var vote = new PollVote();

        // Act
        var json = JsonSerializer.Serialize(vote, JsonBotSerializerContext.Default.PollVote);
        var deserialized = JsonSerializer.Deserialize(json, JsonBotSerializerContext.Default.PollVote);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Null(deserialized.Author);
        Assert.Null(deserialized.OptionIndexes);
        Assert.Equal(0, deserialized.VoteCount);
    }

    [Fact(Timeout = 5000)]
    public void TestPollTerminateSerializationAndDeserialization()
    {
        // Arrange
        var terminate = new PollTerminate
        {
            TargetSent = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        // Act
        var json = JsonSerializer.Serialize(terminate, JsonBotSerializerContext.Default.PollTerminate);
        var deserialized = JsonSerializer.Deserialize(json, JsonBotSerializerContext.Default.PollTerminate);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal(terminate.TargetSent, deserialized.TargetSent);
    }

    [Fact(Timeout = 5000)]
    public void TestPollCreateSerializationAndDeserialization()
    {
        // Arrange
        var create = new PollCreate
        {
            AllowMultiple = true,
            Options = ["Option A", "Option B", "Option C"],
            Question = "Which do you prefer?"
        };

        // Act
        var json = JsonSerializer.Serialize(create, JsonBotSerializerContext.Default.PollCreate);
        var deserialized = JsonSerializer.Deserialize(json, JsonBotSerializerContext.Default.PollCreate);

        // Assert
        Assert.NotNull(deserialized);
        Assert.True(deserialized.AllowMultiple);
        Assert.Equal(3, deserialized.Options!.Count);
        Assert.Equal("Which do you prefer?", deserialized.Question);
    }

    [Fact(Timeout = 5000)]
    public void TestRealPollVoteDeserialization()
    {
        // Arrange
        const string json = """
                            {
                              "author": "Bob",
                              "authorNumber": "+49123",
                              "authorUuid": "11111111-2222-3333-4444-555555555555",
                              "optionIndexes": [1, 3],
                              "targetSentTimestamp": 1704067200000,
                              "voteCount": 8
                            }
                            """;

        // Act
        var result = JsonSerializer.Deserialize(json, JsonBotSerializerContext.Default.PollVote);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Bob", result.Author);
        Assert.Equal(Guid.Parse("11111111-2222-3333-4444-555555555555"), result.AuthorId);
        Assert.Equal(DateTime.UnixEpoch.AddMilliseconds(1704067200000), result.TargetSent);
        Assert.Equal(8, result.VoteCount);
    }
}
