using System.Net;
using System.Text.Json;
using Signal.Bot.IntegrationTests.Utils;
using Signal.Bot.Types;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Signal.Bot.IntegrationTests.Extensions;

public class AccountTests : IntegrationTestBase
{
    [Fact(Timeout = 15000)]
    public async Task GetAccounts_ShouldReturnList()
    {
        // Arrange
        var accounts = new[] { BotNumber };
        MockServer
            .Given(Request.Create()
                .WithPath("/v1/accounts")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithBodyAsJson(accounts));

        // Act
        var result = await Client.GetAccountsAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Contains(BotNumber, result);
    }

    [Fact(Timeout = 15000)]
    public async Task SetPin_ShouldSucceed()
    {
        // Arrange
        const string pin = "1234";
        MockServer
            .Given(Request.Create()
                .WithPath($"/v1/accounts/{BotNumber}/pin")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK));

        // Act
        await Client.SetPinAsync(pin, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(MockServer.LogEntries);
    }

    [Fact(Timeout = 15000)]
    public async Task SetUsername_ShouldReturnResult()
    {
        // Arrange
        const string username = "testuser";
        var expected = new SetUsername { Username = username, UsernameLink = "link" };
        var json = JsonSerializer.Serialize(expected);
        MockServer
            .Given(Request.Create()
                .WithPath(path => path.Contains("/username"))
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody(json));

        // Act
        var result = await Client.SetUsernameAsync(username, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(username, result.Username);
    }

    [Fact(Timeout = 15000)]
    public async Task GetQrCodeLink_ShouldReturnLink()
    {
        // Arrange
        const string expectedLink = "tsdevice:/?uuid=123";
        MockServer
            .Given(Request.Create()
                .WithPath("/v1/qrcodelink")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithBody(expectedLink));

        // Act
        var result =
            await Client.GetQrCodeLinkAsync("test-device", cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(expectedLink, result);
    }
}