using NSubstitute;
using Signal.Bot.UnitTests.Utils;

namespace Signal.Bot.UnitTests.Extensions;

public class GroupTests : BotTestBase
{
    [Fact(Timeout = 5000)]
    public async Task GetGroupsAsync_CallsHttpClient()
    {
        SetupJsonResponse("[]");

        _ = await Client.GetGroupsAsync(cancellationToken: TestContext.Current.CancellationToken);

        await HttpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task CreateGroupAsync_CallsHttpClient()
    {
        SetupJsonResponse();

        _ = await Client.CreateGroupAsync(_ => { }, cancellationToken: TestContext.Current.CancellationToken);

        await HttpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task GetGroupAsync_CallsHttpClient()
    {
        SetupJsonResponse();

        _ = await Client.GetGroupAsync("group-id", cancellationToken: TestContext.Current.CancellationToken);

        await HttpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task UpdateGroupAsync_CallsHttpClient()
    {
        SetupResponse();

        await Client.UpdateGroupAsync("group-id", _ => { }, cancellationToken: TestContext.Current.CancellationToken);

        await HttpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task RemoveGroupAsync_CallsHttpClient()
    {
        SetupResponse();

        await Client.RemoveGroupAsync("group-id", cancellationToken: TestContext.Current.CancellationToken);

        await HttpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task AddGroupAdminAsync_CallsHttpClient()
    {
        SetupResponse();

        await Client.AddGroupAdminAsync("group-id", new[] { "+100" }, cancellationToken: TestContext.Current.CancellationToken);

        await HttpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task RemoveGroupAdminAsync_CallsHttpClient()
    {
        SetupResponse();

        await Client.RemoveGroupAdminAsync("group-id", new List<string> { "+100" }, cancellationToken: TestContext.Current.CancellationToken);

        await HttpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task BlockGroupAsync_CallsHttpClient()
    {
        SetupResponse();

        await Client.BlockGroupAsync("group-id", cancellationToken: TestContext.Current.CancellationToken);

        await HttpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task JoinGroupAsync_CallsHttpClient()
    {
        SetupResponse();

        await Client.JoinGroupAsync("group-id", cancellationToken: TestContext.Current.CancellationToken);

        await HttpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task AddGroupMemberAsync_CallsHttpClient()
    {
        SetupResponse();

        await Client.AddGroupMemberAsync("group-id", new List<string> { "+100" }, cancellationToken: TestContext.Current.CancellationToken);

        await HttpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task RemoveGroupMemberAsync_CallsHttpClient()
    {
        SetupResponse();

        await Client.RemoveGroupMemberAsync("group-id", new List<string> { "+100" }, cancellationToken: TestContext.Current.CancellationToken);

        await HttpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task QuitGroupAsync_CallsHttpClient()
    {
        SetupResponse();

        await Client.QuitGroupAsync("group-id", cancellationToken: TestContext.Current.CancellationToken);

        await HttpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }
}

