using System.Net;
using Signal.Bot.IntegrationTests.Utils;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Signal.Bot.IntegrationTests.Extensions;

public class AccountExtendedTests : IntegrationTestBase
{
    [Fact(Timeout = 5000)]
    public async Task RemovePin_ShouldSucceed()
    {
        // Arrange
        MockServer
            .Given(Request.Create()
                .WithPath(path => path.Contains("/accounts") && path.EndsWith("/pin"))
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK));

        // Act
        await Client.RemovePinAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(MockServer.LogEntries);
    }

    [Fact(Timeout = 5000)]
    public async Task RateLimitChallenge_ShouldSucceed()
    {
        // Arrange
        MockServer
            .Given(Request.Create()
                .WithPath(path => path.Contains("/accounts") && path.Contains("/rate-limit-challenge"))
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK));

        // Act
        await Client.RateLimitChallengeAsync("challenge-token", "captcha-value",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(MockServer.LogEntries);
    }

    [Fact(Timeout = 5000)]
    public async Task UpdateAccountSettings_ShouldSucceed()
    {
        // Arrange
        MockServer
            .Given(Request.Create()
                .WithPath(path => path.Contains("/accounts") && path.EndsWith("/settings"))
                .UsingPut())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK));

        // Act
        await Client.UpdateAccountSettingsAsync(discoverableByNumber: false, shareNumber: false,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(MockServer.LogEntries);
    }

    [Fact(Timeout = 5000)]
    public async Task RemoveUsername_ShouldSucceed()
    {
        // Arrange
        MockServer
            .Given(Request.Create()
                .WithPath(path => path.Contains("/accounts") && path.EndsWith("/username"))
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody("{}"));

        // Act
        await Client.RemoveUsernameAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(MockServer.LogEntries);
    }
}
