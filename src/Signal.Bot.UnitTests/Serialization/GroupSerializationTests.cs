using System.Text.Json;
using Signal.Bot.Requests;

namespace Signal.Bot.UnitTests.Serialization;

public class GroupSerializationTests
{
    [Fact(Timeout = 5000)]
    public void TestAddGroupMemberRequestSerializationAndDeserialization()
    {
        // Arrange
        var addGroupMemberRequest = new AddGroupMemberRequest("", "")
        {
            Members = ["memberUuid456"]
        };

        // Act
        var json = JsonSerializer.Serialize(addGroupMemberRequest);
        var deserializedAddGroupMemberRequest = JsonSerializer.Deserialize<AddGroupMemberRequest>(json);

        // Assert
        Assert.NotNull(deserializedAddGroupMemberRequest);
        Assert.NotNull(deserializedAddGroupMemberRequest.Members);
        Assert.NotEmpty(deserializedAddGroupMemberRequest.Members);
        Assert.Contains(addGroupMemberRequest.Members.ToArray(), deserializedAddGroupMemberRequest.Members.ToArray());
    }

    [Fact(Timeout = 5000)]
    public void TestRemoveGroupMemberRequestSerializationAndDeserialization()
    {
        // Arrange
        var removeGroupMemberRequest = new RemoveGroupMemberRequest("", "")
        {
            Members = ["members"]
        };

        // Act
        var json = JsonSerializer.Serialize(removeGroupMemberRequest);
        var deserializedRemoveGroupMemberRequest = JsonSerializer.Deserialize<RemoveGroupMemberRequest>(json);

        // Assert
        Assert.NotNull(deserializedRemoveGroupMemberRequest);
        Assert.NotNull(deserializedRemoveGroupMemberRequest.Members);
        Assert.Contains(removeGroupMemberRequest.Members.ToArray(),
            deserializedRemoveGroupMemberRequest.Members.ToArray());
    }
}

