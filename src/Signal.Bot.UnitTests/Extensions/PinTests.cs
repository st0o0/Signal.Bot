using NSubstitute;
using Signal.Bot.UnitTests.Utils;

namespace Signal.Bot.UnitTests.Extensions;

public class PinTests : BotTestBase
{
    [Fact(Timeout = 5000)]
    public async Task SetPinAsync_ValidPin_CallsHttpClient()
    {
        // Arrange
        SetupResponse();

        // Act
        await Client.SetPinAsync("123456", cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        await HttpClientMock
            .Received(1)
            .SendAsync(
                Arg.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post),
                Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task SetPinAsync_EmptyPin_StillCallsHttpClient()
    {
        // Arrange
        SetupResponse();

        // Act
        await Client.SetPinAsync("", cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        await HttpClientMock
            .Received(1)
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task RemovePinAsync_ValidRequest_CallsHttpClient()
    {
        // Arrange
        SetupResponse();

        // Act
        await Client.RemovePinAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        await HttpClientMock
            .Received(1)
            .SendAsync(
                Arg.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Delete),
                Arg.Any<CancellationToken>());
    }
}
