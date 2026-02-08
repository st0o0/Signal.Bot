using Signal.Bot.Requests;

namespace Signal.Bot.UnitTests.Requests;

public class CreateGroupBuilderTests
{
    private const string TestNumber = "+1234567890";

    [Fact(Timeout = 5000)]
    public void Constructor_SetsDefaultPermissions()
    {
        // Arrange & Act
        var builder = new CreateGroupBuilder(TestNumber);
        var request = builder.Build();

        // Assert
        Assert.Equal(TestNumber, request.Number);
        Assert.NotNull(request.Permissions);
        Assert.Equal(GroupPermission.OnlyAdmins, request.Permissions.AddMembers);
        Assert.Equal(GroupPermission.OnlyAdmins, request.Permissions.EditGroup);
        Assert.Equal(GroupPermission.OnlyAdmins, request.Permissions.SendMessages);
    }

    [Fact(Timeout = 5000)]
    public void WithName_SetsName()
    {
        // Arrange
        var builder = new CreateGroupBuilder(TestNumber);
        const string name = "Test Group";

        // Act
        var request = builder.WithName(name).Build();

        // Assert
        Assert.Equal(name, request.Name);
    }

    [Fact(Timeout = 5000)]
    public void WithDescription_SetsDescription()
    {
        // Arrange
        var builder = new CreateGroupBuilder(TestNumber);
        const string description = "Test Description";

        // Act
        var request = builder.WithDescription(description).Build();

        // Assert
        Assert.Equal(description, request.Description);
    }

    [Fact(Timeout = 5000)]
    public void WithExpirationTime_SetsExpirationTime()
    {
        // Arrange
        var builder = new CreateGroupBuilder(TestNumber);
        const int expiration = 3600;

        // Act
        var request = builder.WithExpirationTime(expiration).Build();

        // Assert
        Assert.Equal(expiration, request.ExpirationTime);
    }

    [Fact(Timeout = 5000)]
    public void WithGroupLink_SetsGroupLink()
    {
        // Arrange
        var builder = new CreateGroupBuilder(TestNumber);
        const GroupLink link = GroupLink.EnabledWithApproval;

        // Act
        var request = builder.WithGroupLink(link).Build();

        // Assert
        Assert.Equal(link, request.GroupLink);
    }

    [Fact(Timeout = 5000)]
    public void WithAddMemberPermission_UpdatesPermission()
    {
        // Arrange
        var builder = new CreateGroupBuilder(TestNumber);

        // Act
        var request = builder.WithAddMemberPermission(GroupPermission.EveryMember).Build();

        // Assert
        Assert.Equal(GroupPermission.EveryMember, request.Permissions!.AddMembers);
    }

    [Fact(Timeout = 5000)]
    public void WithEditGroupPermission_UpdatesPermission()
    {
        // Arrange
        var builder = new CreateGroupBuilder(TestNumber);

        // Act
        var request = builder.WithEditGroupPermission(GroupPermission.EveryMember).Build();

        // Assert
        Assert.Equal(GroupPermission.EveryMember, request.Permissions!.EditGroup);
    }

    [Fact(Timeout = 5000)]
    public void WithSendMessagesPermission_UpdatesPermission()
    {
        // Arrange
        var builder = new CreateGroupBuilder(TestNumber);

        // Act
        var request = builder.WithSendMessagesPermission(GroupPermission.EveryMember).Build();

        // Assert
        Assert.Equal(GroupPermission.EveryMember, request.Permissions!.SendMessages);
    }

    [Fact(Timeout = 5000)]
    public void WithMembers_SetsMembers()
    {
        // Arrange
        var builder = new CreateGroupBuilder(TestNumber);
        var members = new[] { "+111", "+222" };

        // Act
        var request = builder.WithMembers(members).Build();

        // Assert
        Assert.NotNull(request.Members);
        Assert.Equal(2, request.Members.Length);
        Assert.Contains("+111", request.Members);
        Assert.Contains("+222", request.Members);
    }
}
