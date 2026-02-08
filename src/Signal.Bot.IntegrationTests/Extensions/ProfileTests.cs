using System.Net;
using Signal.Bot.IntegrationTests.Utils;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Signal.Bot.IntegrationTests.Extensions;

public class ProfileIntegrationTests : IntegrationTestBase
{
    [Fact(Timeout = 15000)]
    public async Task UpdateProfile_ShouldSucceed()
    {
        // Arrange
        const string name = "Bot Name";
        const string about = "I am a bot";

        MockServer
            .Given(Request.Create()
                .WithPath($"/v1/profiles/{BotNumber}")
                .UsingPost()
                .WithBody(new JsonMatcher(new
                {
                    name,
                    about
                })))
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithBody("{}"));

        // Act
        await Client.UpdateProfileAsync(name: name, about: about,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(MockServer.LogEntries);
    }

    [Fact(Timeout = 15000)]
    public async Task UpdateProfile_WithAvatar_ShouldSucceed()
    {
        // Arrange
        var avatar = new byte[] { 0x01, 0x02, 0x03 };

        MockServer
            .Given(Request.Create()
                .WithPath($"/v1/profiles/{BotNumber}")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithBody("{}"));

        // Act
        await Client.UpdateProfileAsync(avatar: avatar,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(MockServer.LogEntries);
    }
}

