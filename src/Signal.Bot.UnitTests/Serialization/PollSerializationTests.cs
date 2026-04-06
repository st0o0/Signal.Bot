using System.Text.Json;
using Signal.Bot.Requests;

namespace Signal.Bot.UnitTests.Serialization;
public class PollSerializationTests
{
    [Fact(Timeout = 5000)]
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
        Assert.NotNull(deserialized.AllowMultipleSelections);
        Assert.Equal(true, deserialized.AllowMultipleSelections);
        Assert.NotNull(deserialized.Answers);
        Assert.Equal("yes", deserialized.Answers[0]);
        Assert.Equal("no", deserialized.Answers[1]);
        Assert.Equal("maybe", deserialized.Answers[2]);
        Assert.NotNull(deserialized.Question);
        Assert.Equal("Does this test succeed?", deserialized.Question);
        Assert.NotNull(deserialized.Recipient);
        Assert.Equal("123456789", deserialized.Recipient);
    }

    [Fact(Timeout = 5000)]
    public void TestClosePollRequestSerializationAndDeserialization()
    {
        // Arrange
        var timestamp = DateTime.Now;
        var closePollRequest = new ClosePollRequest("")
        {
            Timestamp = timestamp,
            Recipient = "123456789"
        };
        
        // Act
        var json =  JsonSerializer.Serialize(closePollRequest);
        var deserialized = JsonSerializer.Deserialize<ClosePollRequest>(json);
        
        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal(timestamp, deserialized.Timestamp);
        Assert.NotNull(deserialized.Recipient);
        Assert.Equal("123456789", deserialized.Recipient);
    }

    [Fact(Timeout = 5000)]
    public void TestVotePollRequestSerializationAndDeserialization_SingleAnswer()
    {
        // Arrange
        var timestamp = DateTime.Now;
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
        Assert.Equal(timestamp, deserialized.Timestamp);
        Assert.NotNull(deserialized.SelectedAnswers);
        Assert.Equal(0, deserialized.SelectedAnswers[0]);
        Assert.NotNull(deserialized.PollAuthor);
        Assert.Equal("98765421", deserialized.PollAuthor);
        Assert.NotNull(deserialized.Recipient);
        Assert.Equal("123456789", deserialized.Recipient);
    }
    
    

    [Fact(Timeout = 5000)]
    public void TestVotePollRequestSerializationAndDeserialization_MultipleAnswers()
    {
        // Arrange
        var timestamp = DateTime.Now;
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
        Assert.Equal(timestamp, deserialized.Timestamp);
        Assert.NotNull(deserialized.SelectedAnswers);
        Assert.Equal(2, deserialized.SelectedAnswers[0]);
        Assert.Equal(0, deserialized.SelectedAnswers[1]);
        Assert.NotNull(deserialized.PollAuthor);
        Assert.Equal("98765421", deserialized.PollAuthor);
        Assert.NotNull(deserialized.Recipient);
        Assert.Equal("123456789", deserialized.Recipient);
    }
}