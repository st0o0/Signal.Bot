using NSubstitute;
using Signal.Bot.UnitTests.Utils;

namespace Signal.Bot.UnitTests.Extensions;

public class UsernameTests : BotTestBase
{
    [Fact(Timeout = 5000)]
    public async Task SetUsernameAsync_CallsHttpClient()
    {
        SetupJsonResponse();

        _ = await Client.SetUsernameAsync("user_name", cancellationToken: TestContext.Current.CancellationToken);

        await HttpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task RemoveUsernameAsync_CallsHttpClient()
    {
        SetupResponse();

        await Client.RemoveUsernameAsync(cancellationToken: TestContext.Current.CancellationToken);

        await HttpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }
}

