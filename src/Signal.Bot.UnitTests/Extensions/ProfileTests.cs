using NSubstitute;
using Signal.Bot.UnitTests.Utils;

namespace Signal.Bot.UnitTests.Extensions;

public class ProfileTests : BotTestBase
{
    [Fact(Timeout = 5000)]
    public async Task UpdateProfileAsync_AllParametersNull_CallsHttpClient()
    {
        // Arrange
        SetupResponse();

        // Act
        await Client.UpdateProfileAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        await HttpClientMock
            .Received(1)
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task UpdateProfileAsync_WithName_CallsHttpClient()
    {
        // Arrange
        SetupResponse();

        // Act
        await Client.UpdateProfileAsync(name: "John Doe", cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        await HttpClientMock
            .Received(1)
            .SendAsync(
                Arg.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post),
                Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task UpdateProfileAsync_WithAbout_CallsHttpClient()
    {
        // Arrange
        SetupResponse();

        // Act
        await Client.UpdateProfileAsync(about: "My status", cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        await HttpClientMock
            .Received(1)
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task UpdateProfileAsync_WithBase64Avatar_CallsHttpClient()
    {
        // Arrange
        SetupResponse();

        var testBytes = new byte[]
        {
            0x00, 0x01, 0x02, 0x10, 0x20,
            0x7F, 0x80, 0xAA, 0xFE, 0xFF
        };

        // Act
        await Client.UpdateProfileAsync(avatar: testBytes, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        await HttpClientMock
            .Received(1)
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }
}
