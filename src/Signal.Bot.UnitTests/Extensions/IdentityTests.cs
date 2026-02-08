using NSubstitute;

namespace Signal.Bot.UnitTests.Extensions;

public class IdentityTests : BotTestBase
{
    [Fact(Timeout = 5000)]
    public async Task GetIdentitiesAsync_CallsHttpClient()
    {
        SetupJsonResponse("[]");

        _ = await Client.GetIdentitiesAsync(cancellationToken: TestContext.Current.CancellationToken);

        await HttpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task TrustIdentityAsync_CallsHttpClient()
    {
        SetupResponse();

        await Client.TrustIdentityAsync("+1234567890", cancellationToken: TestContext.Current.CancellationToken);

        await HttpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }
}

