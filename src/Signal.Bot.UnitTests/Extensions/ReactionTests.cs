using System.Text.Json;
using NSubstitute;
using Signal.Bot.Serialization;
using Signal.Bot.UnitTests.Utils;

namespace Signal.Bot.UnitTests.Extensions;

public class ReactionTests : BotTestBase
{
    [Fact(Timeout = 5000)]
    public async Task AddReactionAsync_CallsHttpClient()
    {
        SetupResponse();

        await Client.AddReactionAsync("👍", "+100", "+100", cancellationToken: TestContext.Current.CancellationToken);

        await HttpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task RemoveReactionAsync_CallsHttpClient()
    {
        var json = JsonSerializer.Serialize("ok", JsonBotAPI.Options);
        SetupJsonResponse(json);

        _ = await Client.RemoveReactionAsync("👍", "+100", "+100", cancellationToken: TestContext.Current.CancellationToken);

        await HttpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }
}

