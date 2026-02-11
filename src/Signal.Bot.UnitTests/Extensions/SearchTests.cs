using NSubstitute;
using Signal.Bot.UnitTests.Utils;

namespace Signal.Bot.UnitTests.Extensions;

public class SearchTests : BotTestBase
{
    [Fact(Timeout = 5000)]
    public async Task SearchNumbersAsync_CallsHttpClient()
    {
        SetupJsonResponse("[]");

        _ = await Client.SearchNumbersAsync(["+111"], cancellationToken: TestContext.Current.CancellationToken);

        await HttpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }
}

