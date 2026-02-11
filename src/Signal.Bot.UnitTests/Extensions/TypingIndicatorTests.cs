using NSubstitute;
using Signal.Bot.UnitTests.Utils;

namespace Signal.Bot.UnitTests.Extensions;

public class TypingIndicatorTests : BotTestBase
{
    [Fact(Timeout = 5000)]
    public async Task SetTypingIndicatorAsync_WithRecipient_CallsHttpClient()
    {
        SetupResponse();

        await Client.SetTypingIndicatorAsync(recipient: "+1234567890",
            cancellationToken: TestContext.Current.CancellationToken);

        await HttpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task SetTypingIndicatorAsync_WithGroupId_CallsHttpClient()
    {
        SetupResponse();

        await Client.SetTypingIndicatorAsync(groupId: "group-id",
            cancellationToken: TestContext.Current.CancellationToken);

        await HttpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task SetTypingIndicatorAsync_IsTypingFalse_CallsHttpClient()
    {
        SetupResponse();

        await Client.SetTypingIndicatorAsync(recipient: "+1234567890", isTyping: false,
            cancellationToken: TestContext.Current.CancellationToken);

        await HttpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }
}
