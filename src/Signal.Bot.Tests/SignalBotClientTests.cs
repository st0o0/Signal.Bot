using NSubstitute;

namespace Signal.Bot.Tests;

public class SignalBotClientTests
{
    private readonly HttpClient _httpClientMock;
    private readonly SignalBotClient _client;

    public SignalBotClientTests()
    {
        _httpClientMock = Substitute.For<HttpClient>();
        _client = new SignalBotClient(new SignalBotClientOptions("123", "http://localhost:8080"), _httpClientMock);
    }

    [Fact]
    public async Task SendMessageAsync_ValidRequest_CallsHttpClient()
    {
        // Arrange
        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK)));

        // Act
        await _client.SendMessageAsync(
            ["+0987654321"],
            "Hello World",
            CancellationToken.None);

        // Assert
        await _httpClientMock
            .Received(1)
            .SendAsync(
                Arg.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post && req.RequestUri!.AbsolutePath.Contains("v2/send")),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SendMessageAsync_NullMessage_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _client.SendMessageAsync(["+0987654321"], null!));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task SendMessageAsync_EmptyMessage_ThrowsArgumentException(string message)
    {
        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await _client.SendMessageAsync(["+0987654321"], message));
    }
}