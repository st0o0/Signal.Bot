using NSubstitute;

namespace Signal.Bot.UnitTests.Extensions;

public class AccountTests : BotTestBase
{
    [Fact(Timeout = 5000)]
    public async Task UpdateAccountSettingsAsync_DefaultSettings_CallsHttpClient()
    {
        SetupResponse();

        await Client.UpdateAccountSettingsAsync(cancellationToken: TestContext.Current.CancellationToken);

        await HttpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task UpdateAccountSettingsAsync_DiscoverableFalse_CallsHttpClient()
    {
        SetupResponse();

        await Client.UpdateAccountSettingsAsync(discoverableByNumber: false,
            cancellationToken: TestContext.Current.CancellationToken);

        await HttpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task GetAccountsAsync_CallsHttpClient()
    {
        SetupJsonResponse("[]");

        _ = await Client.GetAccountsAsync(cancellationToken: TestContext.Current.CancellationToken);

        await HttpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task RateLimitChallengeAsync_CallsHttpClient()
    {
        SetupResponse();

        await Client.RateLimitChallengeAsync("challenge", "captcha", cancellationToken: TestContext.Current.CancellationToken);

        await HttpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }
}
