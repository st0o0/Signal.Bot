using System.Net;
using System.Text.Json;
using Signal.Bot.IntegrationTests.Utils;
using Signal.Bot.Serialization;
using Signal.Bot.Types;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Signal.Bot.IntegrationTests.Extensions;

public class GroupExtendedTests : IntegrationTestBase
{
    private const string GroupId = "group.ckRzaEd4VmRzNnJaASAEsasa";

    [Fact(Timeout = 5000)]
    public async Task GetGroup_ShouldReturnGroup()
    {
        // Arrange
        var group = new Group { Id = GroupId, Name = "Test Group" };
        var json = JsonSerializer.Serialize(group, JsonBotAPI.Options);
        MockServer
            .Given(Request.Create()
                .WithPath(path => path.Contains("/groups") && path.Contains(GroupId))
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody(json));

        // Act
        var result = await Client.GetGroupAsync(GroupId,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Multiple(
            () => Assert.Equal(GroupId, result.Id),
            () => Assert.Equal("Test Group", result.Name));
    }

    [Fact(Timeout = 5000)]
    public async Task UpdateGroup_ShouldSucceed()
    {
        // Arrange
        MockServer
            .Given(Request.Create()
                .WithPath(path => path.Contains("/groups") && path.Contains(GroupId) && !path.Contains("/admins"))
                .UsingPut())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK));

        // Act
        await Client.UpdateGroupAsync(GroupId, x => x.WithName("New Name"),
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(MockServer.LogEntries);
    }

    [Fact(Timeout = 5000)]
    public async Task RemoveGroup_ShouldSucceed()
    {
        // Arrange
        MockServer
            .Given(Request.Create()
                .WithPath(path => path.Contains("/groups") && path.EndsWith(GroupId))
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK));

        // Act
        await Client.RemoveGroupAsync(GroupId,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(MockServer.LogEntries);
    }

    [Fact(Timeout = 5000)]
    public async Task AddGroupAdmin_ShouldSucceed()
    {
        // Arrange
        MockServer
            .Given(Request.Create()
                .WithPath(path => path.Contains("/groups") && path.Contains(GroupId) && path.EndsWith("/admins"))
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK));

        // Act
        await Client.AddGroupAdminAsync(GroupId, ["+491700000000"],
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(MockServer.LogEntries);
    }

    [Fact(Timeout = 5000)]
    public async Task RemoveGroupAdmin_ShouldSucceed()
    {
        // Arrange
        MockServer
            .Given(Request.Create()
                .WithPath(path => path.Contains("/groups") && path.Contains(GroupId) && path.EndsWith("/admins"))
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK));

        // Act
        await Client.RemoveGroupAdminAsync(GroupId, ["+491700000000"],
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(MockServer.LogEntries);
    }

    [Fact(Timeout = 5000)]
    public async Task BlockGroup_ShouldSucceed()
    {
        // Arrange
        MockServer
            .Given(Request.Create()
                .WithPath(path => path.Contains("/groups") && path.Contains(GroupId) && path.EndsWith("/block"))
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK));

        // Act
        await Client.BlockGroupAsync(GroupId,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(MockServer.LogEntries);
    }

    [Fact(Timeout = 5000)]
    public async Task JoinGroup_ShouldSucceed()
    {
        // Arrange
        MockServer
            .Given(Request.Create()
                .WithPath(path => path.Contains("/groups") && path.Contains(GroupId) && path.EndsWith("/join"))
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK));

        // Act
        await Client.JoinGroupAsync(GroupId,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(MockServer.LogEntries);
    }

    [Fact(Timeout = 5000)]
    public async Task QuitGroup_ShouldSucceed()
    {
        // Arrange
        MockServer
            .Given(Request.Create()
                .WithPath(path => path.Contains("/groups") && path.Contains(GroupId) && path.EndsWith("/quit"))
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK));

        // Act
        await Client.QuitGroupAsync(GroupId,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(MockServer.LogEntries);
    }

    [Fact(Timeout = 5000)]
    public async Task PinMessage_ShouldSucceed()
    {
        // Arrange
        var timestamp = DateTime.UtcNow;
        MockServer
            .Given(Request.Create()
                .WithPath(path => path.Contains("/groups") && path.Contains(GroupId) && path.EndsWith("/pin-message"))
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK));

        // Act
        await Client.PinMessageAsync(GroupId, RecipientNumber, timestamp, duration: 3600,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(MockServer.LogEntries);
    }

    [Fact(Timeout = 5000)]
    public async Task UnpinMessage_ShouldSucceed()
    {
        // Arrange
        var timestamp = DateTime.UtcNow;
        MockServer
            .Given(Request.Create()
                .WithPath(path => path.Contains("/groups") && path.Contains(GroupId) && path.EndsWith("/pin-message"))
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK));

        // Act
        await Client.UnpinMessageAsync(GroupId, RecipientNumber, timestamp,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(MockServer.LogEntries);
    }

    [Fact(Timeout = 5000)]
    public async Task GetGroupAvatar_ShouldReturnBytes()
    {
        // Arrange
        var avatarData = new byte[] { 0x89, 0x50, 0x4E, 0x47 };
        MockServer
            .Given(Request.Create()
                .WithPath(path => path.Contains("/groups") && path.Contains(GroupId) && path.EndsWith("/avatar"))
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithBody(avatarData));

        // Act
        var result = await Client.GetGroupAvatarAsync(GroupId,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(avatarData, result);
    }
}
