using NSubstitute;
using Signal.Bot.UnitTests.Utils;

namespace Signal.Bot.UnitTests.Extensions;

public class RegisterNumberTests : BotTestBase
{
    [Fact(Timeout = 5000)]
    public async Task RegisterNumberAsync_WithoutParameters_CallsHttpClient()
    {
        SetupResponse();

        await Client.RegisterNumberAsync(cancellationToken: TestContext.Current.CancellationToken);

        await HttpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task RegisterNumberAsync_WithCaptcha_CallsHttpClient()
    {
        SetupResponse();

        await Client.RegisterNumberAsync(captcha: "captcha-token",
            cancellationToken: TestContext.Current.CancellationToken);

        await HttpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task RegisterNumberAsync_WithVoiceOption_CallsHttpClient()
    {
        SetupResponse();

        await Client.RegisterNumberAsync(useVoice: true, cancellationToken: TestContext.Current.CancellationToken);

        await HttpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }
}
