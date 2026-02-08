using NSubstitute;

namespace Signal.Bot.UnitTests.Extensions;

public class ConfigurationTests : BotTestBase
{
    [Fact(Timeout = 5000)]
    public async Task GetConfigurationAsync_CallsHttpClient()
    {
        SetupJsonResponse();

        await Client.GetConfigurationAsync(cancellationToken: TestContext.Current.CancellationToken);

        await HttpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task SetConfigurationAsync_CallsHttpClient()
    {
        SetupResponse();

        await Client.SetConfigurationAsync("info", cancellationToken: TestContext.Current.CancellationToken);

        await HttpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }
}

