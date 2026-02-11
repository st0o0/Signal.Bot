using System.Text.Json;
using NSubstitute;
using Signal.Bot.Serialization;
using Signal.Bot.UnitTests.Utils;

namespace Signal.Bot.UnitTests.Extensions;

public class VerifyNumberTests : BotTestBase
{
    [Fact(Timeout = 5000)]
    public async Task VerifyNumberAsync_ValidToken_ReturnsVerificationResult()
    {
        const string token = "verification-token";
        const string expectedResult = "verified-token";
        var json = JsonSerializer.Serialize(expectedResult, JsonBotAPI.Options);

        SetupJsonResponse(json);

        var result = await Client.VerifyNumberAsync(token, cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal("verified-token", result);
    }

    [Fact(Timeout = 5000)]
    public async Task VerifyNumberAsync_WithPin_CallsHttpClient()
    {
        const string token = "verification-token";
        const string pin = "123456";
        SetupResponse();

        await Client.VerifyNumberAsync(token, pin, cancellationToken: TestContext.Current.CancellationToken);

        await HttpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }
}
