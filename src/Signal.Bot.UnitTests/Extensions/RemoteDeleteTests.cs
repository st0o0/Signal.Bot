using NSubstitute;

namespace Signal.Bot.UnitTests.Extensions;

public class RemoteDeleteTests : BotTestBase
{
    [Fact(Timeout = 5000)]
    public async Task RemoteDeleteAsync_CallsHttpClient()
    {
        SetupJsonResponse();

        _ = await Client.RemoteDeleteAsync("+199", cancellationToken: TestContext.Current.CancellationToken);

        await HttpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }
}

