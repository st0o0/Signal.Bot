using NSubstitute;

namespace Signal.Bot.UnitTests.Extensions;

public class StickerPackTests : BotTestBase
{
    [Fact(Timeout = 5000)]
    public async Task GetStickerPacksAsync_CallsHttpClient()
    {
        SetupJsonResponse("[]");

        _ = await Client.GetStickerPacksAsync(cancellationToken: TestContext.Current.CancellationToken);

        await HttpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task AddStickerPackAsync_CallsHttpClient()
    {
        SetupResponse();

        await Client.AddStickerPackAsync("pack-id", "pack-key", cancellationToken: TestContext.Current.CancellationToken);

        await HttpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }
}

