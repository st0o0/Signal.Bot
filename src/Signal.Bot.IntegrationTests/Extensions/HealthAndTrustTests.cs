using System.Net;
using System.Text.Json;
using Signal.Bot.IntegrationTests.Utils;
using Signal.Bot.Serialization;
using Signal.Bot.Types;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Signal.Bot.IntegrationTests.Extensions;

public class HealthAndTrustTests : IntegrationTestBase
{
    [Fact(Timeout = 5000)]
    public async Task IsHealthy_WhenApiReturns200_ShouldReturnTrue()
    {
        // Arrange
        MockServer
            .Given(Request.Create()
                .WithPath("/v1/health")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK));

        // Act
        var result = await Client.IsHealthyAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result);
    }

    [Fact(Timeout = 5000)]
    public async Task IsHealthy_WhenApiReturns500_ShouldReturnFalse()
    {
        // Arrange
        MockServer
            .Given(Request.Create()
                .WithPath("/v1/health")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.InternalServerError));

        // Act
        var result = await Client.IsHealthyAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result);
    }

    [Fact(Timeout = 5000)]
    public async Task IsHealthy_WhenNoServerConfigured_ShouldReturnFalse()
    {
        // Act - no mock configured, so the request gets a default non-match response
        var result = await Client.IsHealthyAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result);
    }

    [Fact(Timeout = 5000)]
    public async Task GetTrustMode_ShouldReturnSettings()
    {
        // Arrange
        var settings = new TrustModeSettings { TrustMode = "always" };
        var json = JsonSerializer.Serialize(settings, JsonBotAPI.Options);
        MockServer
            .Given(Request.Create()
                .WithPath(path => path.Contains("/configuration") && path.Contains("/settings"))
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody(json));

        // Act
        var result = await Client.GetTrustModeAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("always", result.TrustMode);
    }

    [Fact(Timeout = 5000)]
    public async Task SetTrustMode_ShouldReturnUpdatedSettings()
    {
        // Arrange
        var settings = new TrustModeSettings { TrustMode = "on-first-use" };
        var json = JsonSerializer.Serialize(settings, JsonBotAPI.Options);
        MockServer
            .Given(Request.Create()
                .WithPath(path => path.Contains("/configuration") && path.Contains("/settings"))
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody(json));

        // Act
        var result = await Client.SetTrustModeAsync("on-first-use",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("on-first-use", result.TrustMode);
    }
}
