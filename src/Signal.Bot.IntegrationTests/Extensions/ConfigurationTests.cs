using System.Net;
using Signal.Bot.IntegrationTests.Utils;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Signal.Bot.IntegrationTests.Extensions;

public class ConfigurationTests : IntegrationTestBase
{
    [Fact(Timeout = 15000)]
    public async Task GetConfiguration_ShouldReturnConfiguration()
    {
        // Arrange
        const string json = "{\"logging\": {\"Level\": \"info\"}}";
        MockServer
            .Given(Request.Create()
                .WithPath("/v1/configuration")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithBody(json));

        // Act
        var result = await Client.GetConfigurationAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("info", result.Logging?.Level);
    }

    [Fact(Timeout = 15000)]
    public async Task GetAbout_ShouldReturnAbout()
    {
        // Arrange
        const string json = "{\"version\": \"1.0.0\"}";
        MockServer
            .Given(Request.Create()
                .WithPath("/v1/about")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithBody(json));

        // Act
        var result = await Client.GetAboutAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("1.0.0", result.Version);
    }

    [Fact(Timeout = 15000)]
    public async Task SetConfiguration_ShouldSucceed()
    {
        // Arrange
        MockServer
            .Given(Request.Create()
                .WithPath("/v1/configuration")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK));

        // Act
        await Client.SetConfigurationAsync("debug", cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(MockServer.LogEntries);
    }
}
