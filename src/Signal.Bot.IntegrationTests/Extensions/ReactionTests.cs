using System.Net;
using Signal.Bot.IntegrationTests.Utils;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Signal.Bot.IntegrationTests.Extensions;

public class ReactionTests : IntegrationTestBase
{
    [Fact(Timeout = 15000)]
    public async Task AddReaction_ShouldSucceed()
    {
        // Arrange
        const string reaction = "👍";
        var timestamp = DateTime.Now;

        MockServer
            .Given(Request.Create()
                .WithPath("/v1/reactions")
                .UsingPost()
                .WithBody(new JsonMatcher(new
                {
                    reaction,
                    recipient = RecipientNumber,
                    target_author = RecipientNumber,
                    timestamp = (long)timestamp.Subtract(DateTime.UnixEpoch).TotalMilliseconds
                })))
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.Created)
                .WithBody("{}"));

        // Act
        await Client.AddReactionAsync(reaction, RecipientNumber, RecipientNumber, timestamp,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(MockServer.LogEntries);
    }

    [Fact(Timeout = 15000)]
    public async Task RemoveReaction_ShouldSucceed()
    {
        // Arrange
        const string reaction = "👍";

        MockServer
            .Given(Request.Create()
                .WithPath("/v1/reactions/*")
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithBodyAsJson("ok"));

        // Act
        var result = await Client.RemoveReactionAsync(reaction, RecipientNumber, RecipientNumber,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal("ok", result);
        Assert.Single(MockServer.LogEntries);
    }
}

