using NSubstitute;

namespace Signal.Bot.UnitTests.Extensions;

public class ReceiptTests : BotTestBase
{
    [Fact(Timeout = 5000)]
    public async Task SendReceiptAsync_CallsHttpClient()
    {
        SetupResponse();

        await Client.SendReceiptAsync("+100", cancellationToken: TestContext.Current.CancellationToken);

        await HttpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }
}

