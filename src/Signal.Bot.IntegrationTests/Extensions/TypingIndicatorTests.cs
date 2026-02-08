using System.Net;
using Signal.Bot.IntegrationTests.Utils;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Signal.Bot.IntegrationTests.Extensions;

public class TypingIndicatorIntegrationTests : IntegrationTestBase
{
    [Fact(Timeout = 15000)]
    public async Task SetTypingIndicator_StartTyping_ShouldUsePut()
    {
        // Arrange
        MockServer
            .Given(Request.Create()
                .WithPath($"/v1/typing-indicator/{BotNumber}")
                .UsingPut())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.Created)
                .WithBody("{}"));

        // Act
        await Client.SetTypingIndicatorAsync(recipient: RecipientNumber, isTyping: true,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(MockServer.LogEntries);
    }

    [Fact(Timeout = 15000)]
    public async Task SetTypingIndicator_StopTyping_ShouldUseDelete()
    {
        // Arrange
        MockServer
            .Given(Request.Create()
                .WithPath($"/v1/typing-indicator/{BotNumber}")
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent));

        // Act
        await Client.SetTypingIndicatorAsync(recipient: RecipientNumber, isTyping: false,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(MockServer.LogEntries);
    }
}

