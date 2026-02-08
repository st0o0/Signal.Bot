using System.Net;
using Signal.Bot.IntegrationTests.Utils;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Signal.Bot.IntegrationTests.Extensions;

public class GroupIntegrationTests : IntegrationTestBase
{
    private const string GroupId = "group.ckRzaEd4VmRzNnJaASAEsasa";

    [Fact(Timeout = 15000)]
    public async Task CreateGroup_WithMembers_ShouldReturnGroupId()
    {
        // Arrange
        var members = new[] { RecipientNumber };
        var groupName = "Test Group";

        MockServer
            .Given(Request.Create()
                .WithPath("/v1/groups/*")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.Created)
                .WithBodyAsJson(new { id = GroupId }));

        // Act
        var result = await Client.CreateGroupAsync(x => x.WithName(groupName).WithMembers(members),
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(MockServer.LogEntries);
    }

    [Fact(Timeout = 15000)]
    public async Task AddGroupMember_ShouldSucceed()
    {
        // Arrange
        var newMember = "+491700000000";

        MockServer
            .Given(Request.Create()
                .WithPath($"/v1/groups/{BotNumber}/{GroupId}/members")
                .UsingPost()
                .WithBody(new JsonMatcher(new
                {
                    members = new[] { newMember }
                })))
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.Created)
                .WithBody("{}"));

        // Act
        await Client.AddGroupMemberAsync(GroupId, new[] { newMember },
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(MockServer.LogEntries);
    }

    [Fact(Timeout = 15000)]
    public async Task RemoveGroupMember_ShouldSucceed()
    {
        // Arrange
        var memberToRemove = "+491700000000";

        MockServer
            .Given(Request.Create()
                .WithPath($"/v1/groups/{BotNumber}/{GroupId}/members")
                .UsingDelete()
                .WithBody(new JsonMatcher(new
                {
                    members = new[] { memberToRemove }
                })))
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithBody("{}"));

        // Act
        await Client.RemoveGroupMemberAsync(GroupId, new[] { memberToRemove },
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(MockServer.LogEntries);
    }

    [Fact(Timeout = 15000)]
    public async Task GetGroups_ShouldReturnList()
    {
        // Arrange
        MockServer
            .Given(Request.Create()
                .WithPath("/v1/groups/*")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithBodyAsJson(new[] { new { id = GroupId, name = "Group 1" } }));

        // Act
        var result = await Client.GetGroupsAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(MockServer.LogEntries);
    }

    [Fact(Timeout = 15000)]
    public async Task CreateGroup_EmptyResponse_ShouldHandleGracefully()
    {
        // Arrange
        MockServer
            .Given(Request.Create()
                .WithPath("/v1/groups/*")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.Created)
                .WithBody(""));

        // Act
        var result = await Client.CreateGroupAsync(x => x.WithName("Test").WithMembers(["+123"]),
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(result);
    }
}

