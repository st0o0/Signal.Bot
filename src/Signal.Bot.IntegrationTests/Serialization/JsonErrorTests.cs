using System.Net;
using Signal.Bot.IntegrationTests.Utils;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Signal.Bot.IntegrationTests.Serialization;

public class JsonErrorIntegrationTests : IntegrationTestBase
{
    [Fact(Timeout = 15000)]
    public async Task ApiReturnsInvalidJson_ShouldReturnNull()
    {
        // Arrange
        MockServer
            .Given(Request.Create()
                .WithPath("/v2/send")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.Created)
                .WithBody("invalid { json"));

        // Act
        var result = await Client.SendMessageAsync("Test", RecipientNumber,
                cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(result);
    }

    [Fact(Timeout = 15000)]
    public async Task ApiReturnsEmptyBody_WhenExpectingJson_ShouldReturnNull()
    {
        // Arrange
        MockServer
            .Given(Request.Create()
                .WithPath("/v2/send")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.Created)
                .WithBody(""));

        // Act
        var result = await Client.SendMessageAsync("Test", RecipientNumber,
                cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(result);
    }

    [Fact(Timeout = 15000)]
    public async Task ApiReturnsUnexpectedJsonStructure_ShouldHandleOrThrow()
    {
        // Arrange
        MockServer
            .Given(Request.Create()
                .WithPath("/v2/send")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.Created)
                .WithBodyAsJson(new { unexpected = "structure" }));

        // Act
        var result = await Client.SendMessageAsync("Test", RecipientNumber,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(default, result.Timestamp);
    }
}

