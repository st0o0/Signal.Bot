using System.Net;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Signal.Bot.Tests;

public class SignalBotEdgeCasesTests : IAsyncDisposable
{
    private readonly WireMockServer _mockServer;
    private readonly SignalBotClient _client;
    private const string BotNumber = "+491701234567";
    private const string RecipientNumber = "+491709876543";

    public SignalBotEdgeCasesTests()
    {
        _mockServer = WireMockServer.Start();
        _client = new SignalBotClient(x => x.WithNumber(BotNumber).WithBaseUrl(_mockServer.Url!));
    }

    public async ValueTask DisposeAsync()
    {
        _mockServer?.Stop();
        _mockServer?.Dispose();
    }

    #region Network & Connection Issues

    [Fact]
    public async Task SendMessage_ConnectionTimeout_ShouldThrowTimeout()
    {
        // Arrange
        _mockServer
            .Given(Request.Create()
                .WithPath("/v2/send")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithDelay(TimeSpan.FromSeconds(30))
                .WithBodyAsJson(new { timestamp = 123456 }));

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));

        // Act & Assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
            await _client.SendMessageAsync(
                message: "Timeout test",
                recipient: RecipientNumber,
                cancellationToken: cts.Token
            ));
    }

    [Fact]
    public async Task SendMessage_ConnectionResetByPeer_ShouldThrow()
    {
        // Arrange
        _mockServer
            .Given(Request.Create()
                .WithPath("/v2/send")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithFault(FaultType.EMPTY_RESPONSE));

        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(async () =>
            await _client.SendMessageAsync(message: "Connection reset test", recipient: RecipientNumber,
                cancellationToken: TestContext.Current.CancellationToken));
    }

    #endregion

    #region JSON Parsing Errors

    //[Fact]
    public async Task ReceiveMessages_InvalidJSON_ShouldThrow()
    {
        // Arrange
        _mockServer
            .Given(Request.Create()
                .WithPath($"/v1/receive/{BotNumber}")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithBody("{ invalid json ]"));

        // Act & Assert
        //await Assert.ThrowsAsync<JsonException>(async () => await _client.ReceiveMessagesAsync(BotNumber));
    }

    //[Fact]
    public async Task ReceiveMessages_NullFields_ShouldHandleGracefully()
    {
        // Arrange
        var messages = new[]
        {
            new
            {
                envelope = new
                {
                    source = RecipientNumber,
                    timestamp = 123456789L,
                    dataMessage = (object?)null
                }
            }
        };

        _mockServer
            .Given(Request.Create()
                .WithPath($"/v1/receive/{BotNumber}")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithBodyAsJson(messages));

        // Act
        //var result = await _client.ReceiveMessagesAsync(BotNumber);

        // Assert
        //Assert.Single(result);
        //Assert.Null(result.First().Envelope.DataMessage);
    }

    #endregion

    #region Unicode & Special Characters

    [Fact]
    public async Task SendMessage_UnicodeEmojis_ShouldHandleCorrectly()
    {
        // Arrange
        var messageWithEmojis = "🎉🎊 Party! 😀😃 👍✌️";

        _mockServer
            .Given(Request.Create()
                .WithPath("/v2/send")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.Created)
                .WithBodyAsJson(new { timestamp = 123456 }));

        // Act
        await _client.SendMessageAsync(message: messageWithEmojis, recipient: RecipientNumber,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(_mockServer.LogEntries);
    }

    [Fact]
    public async Task SendMessage_RightToLeftText_ShouldHandleCorrectly()
    {
        // Arrange - Arabic/Hebrew text
        const string rtlMessage = "مرحبا שלום";

        _mockServer
            .Given(Request.Create()
                .WithPath("/v2/send")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.Created)
                .WithBodyAsJson(new { timestamp = 123456 }));

        // Act
        await _client.SendMessageAsync(message: rtlMessage, recipient: RecipientNumber,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(_mockServer.LogEntries);
    }

    #endregion

    #region Phone Number Validation

    [Theory]
    [InlineData("123456")]
    [InlineData("not-a-number")]
    [InlineData("++491701234567")]
    public async Task RegisterNumber_InvalidFormat_ShouldReturnError(string invalidNumber)
    {
        // Arrange
        _mockServer
            .Given(Request.Create()
                .WithPath($"/v1/register/{invalidNumber}")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.BadRequest)
                .WithBodyAsJson(new { error = "Invalid phone number format" }));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<HttpRequestException>(async () =>
            await _client.RegisterNumberAsync(invalidNumber, cancellationToken: TestContext.Current.CancellationToken));

        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
    }

    #endregion

    #region Message Size Limits

    [Fact]
    public async Task SendMessage_VeryLongMessage_ShouldSucceed()
    {
        // Arrange - Signal allows up to ~64KB messages
        var longMessage = new string('A', 50000);

        _mockServer
            .Given(Request.Create()
                .WithPath("/v2/send")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.Created)
                .WithBodyAsJson(new { timestamp = 123456 }));

        // Act
        await _client.SendMessageAsync(message: longMessage, recipient: RecipientNumber,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(_mockServer.LogEntries);
    }

    [Fact]
    public async Task SendMessage_ExtremelyLongMessage_ShouldReturnError()
    {
        // Arrange
        var tooLongMessage = new string('A', 100000);

        _mockServer
            .Given(Request.Create()
                .WithPath("/v2/send")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.BadRequest)
                .WithBodyAsJson(new { error = "Message too long" }));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<HttpRequestException>(async () =>
            await _client.SendMessageAsync(message: tooLongMessage, recipient: RecipientNumber,
                cancellationToken: TestContext.Current.CancellationToken));

        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
    }

    #endregion

    #region Rate Limiting

    [Fact]
    public async Task SendMessage_RateLimited_ShouldIncludeHeader()
    {
        // Arrange
        _mockServer
            .Given(Request.Create()
                .WithPath("/v2/send")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.TooManyRequests)
                .WithHeader("Retry-After", "60")
                .WithBodyAsJson(new { error = "Rate limit exceeded" }));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<HttpRequestException>(async () =>
            await _client.SendMessageAsync(message: "Rate limit test", recipient: RecipientNumber,
                cancellationToken: TestContext.Current.CancellationToken));

        Assert.Equal(HttpStatusCode.TooManyRequests, exception.StatusCode);
    }

    #endregion

    #region HTTP Error Codes

    [Theory]
    [InlineData(HttpStatusCode.Unauthorized)]
    [InlineData(HttpStatusCode.Forbidden)]
    [InlineData(HttpStatusCode.NotFound)]
    [InlineData(HttpStatusCode.InternalServerError)]
    [InlineData(HttpStatusCode.ServiceUnavailable)]
    public async Task ApiCall_VariousHttpErrors_ShouldThrowWithCorrectCode(HttpStatusCode statusCode)
    {
        // Arrange
        _mockServer
            .Given(Request.Create()
                .WithPath("/v2/send")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(statusCode)
                .WithBodyAsJson(new { error = "Error" }));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<HttpRequestException>(async () =>
            await _client.SendMessageAsync(message: "Error test", recipient: RecipientNumber,
                cancellationToken: TestContext.Current.CancellationToken));

        Assert.Equal(statusCode, exception.StatusCode);
    }

    #endregion

    #region Empty Response Handling

    // [Fact]
    // public async Task ReceiveMessages_EmptyArray_ShouldReturnEmpty()
    // {
    //     // Arrange
    //     _mockServer
    //         .Given(Request.Create()
    //             .WithPath($"/v1/receive/{BotNumber}")
    //             .UsingGet())
    //         .RespondWith(Response.Create()
    //             .WithStatusCode(HttpStatusCode.OK)
    //             .WithBodyAsJson(Array.Empty<object>()));
    //
    //     // Act
    //     var result = await _client.ReceiveMessagesAsync(BotNumber);
    //
    //     // Assert
    //     Assert.Empty(result);
    // }

    [Fact]
    public async Task ListGroups_NoGroups_ShouldReturnEmpty()
    {
        // Arrange
        _mockServer
            .Given(Request.Create()
                .WithPath($"/v1/groups/{BotNumber}")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithBodyAsJson(Array.Empty<object>()));

        // Act
        var result = await _client.GetGroupsAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Empty(result);
    }

    #endregion
}