using System.Text.Json;
using Signal.Bot.Requests;

namespace Signal.Bot.UnitTests.Serialization;

public class PollSerializationTests
{
    [Fact]
    public void TestAddPollRequestSerializationAndDeserialization()
    {
        // Arrange
        var addPollRequest = new AddPollRequest("")
        {
            AllowMultipleSelections = true,
            Answers = ["yes", "no", "maybe"],
            Question = "Does this test succeed?",
            Recipient = "123456789"
        };

        // Act
        var json = JsonSerializer.Serialize(addPollRequest);
        var deserialized = JsonSerializer.Deserialize<AddPollRequest>(json);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Multiple(
            () => Assert.True(deserialized.AllowMultipleSelections!.Value),
            () => Assert.Equal(["yes", "no", "maybe"], deserialized.Answers),
            () => Assert.Equal("Does this test succeed?", deserialized.Question),
            () => Assert.Equal("123456789", deserialized.Recipient));
    }

    [Fact]
    public void TestClosePollRequestSerializationAndDeserialization()
    {
        // Arrange
        var timestamp = TimeProvider.System.GetUtcNow().DateTime;
        var closePollRequest = new ClosePollRequest("")
        {
            Timestamp = timestamp,
            Recipient = "123456789"
        };

        // Act
        var json = JsonSerializer.Serialize(closePollRequest);
        var deserialized = JsonSerializer.Deserialize<ClosePollRequest>(json);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Multiple(
            () => Assert.Equal(timestamp, deserialized.Timestamp),
            () => Assert.Equal("123456789", deserialized.Recipient));
    }

    [Fact]
    public void TestVotePollRequestSerializationAndDeserialization_SingleAnswer()
    {
        // Arrange
        var timestamp = TimeProvider.System.GetUtcNow().DateTime;
        var votePollRequest = new VotePollRequest("")
        {
            Recipient = "123456789",
            Timestamp = timestamp,
            SelectedAnswers = [0],
            PollAuthor = "98765421"
        };

        // Act
        var json = JsonSerializer.Serialize(votePollRequest);
        var deserialized = JsonSerializer.Deserialize<VotePollRequest>(json);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Multiple(
            () => Assert.Equal(timestamp, deserialized.Timestamp),
            () => Assert.Equal([0], deserialized.SelectedAnswers),
            () => Assert.Equal("98765421", deserialized.PollAuthor),
            () => Assert.Equal("123456789", deserialized.Recipient));
    }

    [Fact]
    public void TestVotePollRequestSerializationAndDeserialization_MultipleAnswers()
    {
        // Arrange
        var timestamp = TimeProvider.System.GetUtcNow().DateTime;
        var votePollRequest = new VotePollRequest("")
        {
            Recipient = "123456789",
            Timestamp = timestamp,
            SelectedAnswers = [2, 0],
            PollAuthor = "98765421"
        };

        // Act
        var json = JsonSerializer.Serialize(votePollRequest);
        var deserialized = JsonSerializer.Deserialize<VotePollRequest>(json);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Multiple(
            () => Assert.Equal(timestamp, deserialized.Timestamp),
            () => Assert.Equal([2, 0], deserialized.SelectedAnswers),
            () => Assert.Equal("98765421", deserialized.PollAuthor),
            () => Assert.Equal("123456789", deserialized.Recipient));
    }
}
