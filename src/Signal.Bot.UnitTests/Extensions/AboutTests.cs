using System.Text.Json;
using NSubstitute;
using Signal.Bot.Serialization;
using Signal.Bot.Types;
using Signal.Bot.UnitTests.Utils;

namespace Signal.Bot.UnitTests.Extensions;

public class AboutTests : BotTestBase
{
    [Fact(Timeout = 5000)]
    public async Task GetAboutAsync_ValidRequest_CallsHttpClient()
    {
        // Arrange
        var about = new About { Version = "1.0.0" };
        var json = JsonSerializer.Serialize(about, JsonBotAPI.Options);

        SetupJsonResponse(json);

        // Act
        var result = await Client.GetAboutAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        await HttpClientMock
            .Received(1)
            .SendAsync(
                Arg.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get),
                Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task GetAboutAsync_WithCancellationToken_PassesCancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        var about = new About { Version = "1.0.0" };
        var json = JsonSerializer.Serialize(about, JsonBotAPI.Options);

        SetupJsonResponse(json);

        // Act
        await Client.GetAboutAsync(cts.Token);

        // Assert
        await HttpClientMock
            .Received(1)
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());

        cts.Dispose();
    }
}
