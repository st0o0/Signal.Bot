using Signal.Bot.Requests;

namespace Signal.Bot.UnitTests.Requests;

public class UpdateGroupBuilderTests
{
    private const string TestNumber = "+1234567890";
    private const string TestGroupId = "group-id-123";

    [Fact(Timeout = 5000)]
    public void Constructor_SetsDefaultPermissions()
    {
        // Arrange & Act
        var builder = new UpdateGroupBuilder(TestNumber, TestGroupId);
        var request = builder.Build();

        // Assert
        Assert.Equal(TestNumber, request.Number);
        Assert.Equal(TestGroupId, request.GroupId);
        Assert.NotNull(request.Permissions);
        Assert.Equal(GroupPermission.OnlyAdmins, request.Permissions.AddMembers);
        Assert.Equal(GroupPermission.OnlyAdmins, request.Permissions.EditGroup);
        Assert.Equal(GroupPermission.OnlyAdmins, request.Permissions.SendMessages);
    }

    [Fact(Timeout = 5000)]
    public void WithName_SetsName()
    {
        // Arrange
        var builder = new UpdateGroupBuilder(TestNumber, TestGroupId);
        const string name = "Updated Group";

        // Act
        var request = builder.WithName(name).Build();

        // Assert
        Assert.Equal(name, request.Name);
    }

    [Fact(Timeout = 5000)]
    public void WithDescription_SetsDescription()
    {
        // Arrange
        var builder = new UpdateGroupBuilder(TestNumber, TestGroupId);
        const string description = "Updated Description";

        // Act
        var request = builder.WithDescription(description).Build();

        // Assert
        Assert.Equal(description, request.Description);
    }

    [Fact(Timeout = 5000)]
    public void WithExpirationTime_SetsExpirationTime()
    {
        // Arrange
        var builder = new UpdateGroupBuilder(TestNumber, TestGroupId);
        const int expiration = 7200;

        // Act
        var request = builder.WithExpirationTime(expiration).Build();

        // Assert
        Assert.Equal(expiration, request.ExpirationTime);
    }

    [Fact(Timeout = 5000)]
    public void WithGroupLink_SetsGroupLink()
    {
        // Arrange
        var builder = new UpdateGroupBuilder(TestNumber, TestGroupId);
        const GroupLink link = GroupLink.Disabled;

        // Act
        var request = builder.WithGroupLink(link).Build();

        // Assert
        Assert.Equal(link, request.GroupLink);
    }

    [Fact(Timeout = 5000)]
    public void WithAddMemberPermission_UpdatesPermission()
    {
        // Arrange
        var builder = new UpdateGroupBuilder(TestNumber, TestGroupId);

        // Act
        var request = builder.WithAddMemberPermission(GroupPermission.EveryMember).Build();

        // Assert
        Assert.Equal(GroupPermission.EveryMember, request.Permissions!.AddMembers);
    }

    [Fact(Timeout = 5000)]
    public void WithEditGroupPermission_UpdatesPermission()
    {
        // Arrange
        var builder = new UpdateGroupBuilder(TestNumber, TestGroupId);

        // Act
        var request = builder.WithEditGroupPermission(GroupPermission.EveryMember).Build();

        // Assert
        Assert.Equal(GroupPermission.EveryMember, request.Permissions!.EditGroup);
    }

    [Fact(Timeout = 5000)]
    public void WithSendMessagesPermission_UpdatesPermission()
    {
        // Arrange
        var builder = new UpdateGroupBuilder(TestNumber, TestGroupId);

        // Act
        var request = builder.WithSendMessagesPermission(GroupPermission.EveryMember).Build();

        // Assert
        Assert.Equal(GroupPermission.EveryMember, request.Permissions!.SendMessages);
    }

    [Fact(Timeout = 5000)]
    public void WithAvatar_SetsAvatar()
    {
        // Arrange
        var builder = new UpdateGroupBuilder(TestNumber, TestGroupId);
        var avatar = new byte[] { 0x01, 0x02, 0x03 };

        // Act
        var request = builder.WithAvatar(avatar).Build();

        // Assert
        Assert.Equal(Base64String.FromBytes(avatar), request.Avatar);
    }
}
