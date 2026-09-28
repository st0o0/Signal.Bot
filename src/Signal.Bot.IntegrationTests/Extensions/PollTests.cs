using System.Net;
using System.Text.Json;
using Signal.Bot.IntegrationTests.Utils;
using Signal.Bot.Serialization;
using Signal.Bot.Types;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Signal.Bot.IntegrationTests.Extensions;

public class PollTests : IntegrationTestBase
{
    [Fact(Timeout = 15000)]
    public async Task AddPoll_ShouldReturnPollResponse()
    {
        // Arrange
        var pollResponse = new PollResponse { Timestamp = DateTime.UtcNow };
        var json = JsonSerializer.Serialize(pollResponse, JsonBotAPI.Options);
        MockServer
            .Given(Request.Create()
                .WithPath(path => path.Contains("/polls") && !path.Contains("/vote"))
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.Created)
                .WithHeader("Content-Type", "application/json")
                .WithBody(json));

        // Act
        var result = await Client.AddPollAsync(
            allowMultipleSelections: false,
            answers: ["Yes", "No"],
            question: "Do you agree?",
            recipient: RecipientNumber,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Single(MockServer.LogEntries);
    }

    [Fact(Timeout = 15000)]
    public async Task ClosePoll_ShouldSucceed()
    {
        // Arrange
        var timestamp = DateTime.UtcNow;
        MockServer
            .Given(Request.Create()
                .WithPath(path => path.Contains("/polls") && !path.Contains("/vote"))
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK));

        // Act
        await Client.ClosePollAsync(timestamp, RecipientNumber,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(MockServer.LogEntries);
    }

    [Fact(Timeout = 15000)]
    public async Task VotePoll_ShouldSucceed()
    {
        // Arrange
        var timestamp = DateTime.UtcNow;
        MockServer
            .Given(Request.Create()
                .WithPath(path => path.Contains("/polls") && path.EndsWith("/vote"))
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK));

        // Act
        await Client.VotePollAsync(RecipientNumber, timestamp, RecipientNumber,
            selectedAnswers: [0],
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(MockServer.LogEntries);
    }
}
