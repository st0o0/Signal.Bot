using System.Net;
using System.Text.Json;
using Signal.Bot.IntegrationTests.Utils;
using Signal.Bot.Serialization;
using Signal.Bot.Types;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Signal.Bot.IntegrationTests.Extensions;

public class MessageTests : IntegrationTestBase
{
    [Fact(Timeout = 5000)]
    public async Task SendMessage_SimpleText_ShouldReturnTimestamp()
    {
        // Arrange
        var dateTime = DateTimeOffset.UtcNow;
        var timestamp = dateTime.ToUnixTimeMilliseconds();

        MockServer
            .Given(Request.Create()
                .WithPath("/v2/send")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.Created)
                .WithHeader("Content-Type", "application/json")
                .WithBodyAsJson(new { timestamp }));

        // Act
        var result = await Client.SendMessageAsync("Hello World!", RecipientNumber,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);

        var expectedLocal = DateTimeOffset.FromUnixTimeMilliseconds(timestamp).DateTime;
        Assert.InRange(result.Timestamp, expectedLocal.AddSeconds(-1), expectedLocal.AddSeconds(1));
    }

    [Theory(Timeout = 5000)]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.Unauthorized)]
    [InlineData(HttpStatusCode.NotFound)]
    [InlineData(HttpStatusCode.TooManyRequests)]
    public async Task SendMessage_ApiErrors_ShouldReturnNull(HttpStatusCode statusCode)
    {
        // Arrange
        MockServer
            .Given(Request.Create()
                .WithPath("/v2/send")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(statusCode)
                .WithBodyAsJson(new { error = "Error occurred" }));

        // Act
        var result = await Client.SendMessageAsync("Test", RecipientNumber,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(result);
    }

    [Fact(Timeout = 5000)]
    public async Task SendMessage_UnicodeEmojis_ShouldHandleCorrectly()
    {
        // Arrange
        const string messageWithEmojis = "🎉🎊 Party! 😀😃 👍✌️";

        MockServer
            .Given(Request.Create()
                .WithPath("/v2/send")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.Created)
                .WithBodyAsJson(new { timestamp = 123456 }));

        // Act
        await Client.SendMessageAsync(message: messageWithEmojis, recipient: RecipientNumber,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        var entry = Assert.Single(MockServer.LogEntries);
        Assert.NotNull(entry.RequestMessage);
        Assert.NotNull(entry.RequestMessage.Body);
        var request = JsonSerializer.Deserialize(entry.RequestMessage.Body, JsonBotSerializerContext.Default.SendMessageRequest);
        Assert.Equal(messageWithEmojis, request!.Message);
    }

    [Fact(Timeout = 5000)]
    public async Task SendMessage_RightToLeftText_ShouldHandleCorrectly()
    {
        // Arrange
        const string rtlMessage = "مرحبا שלום";

        MockServer
            .Given(Request.Create()
                .WithPath("/v2/send")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.Created)
                .WithBodyAsJson(new { timestamp = 123456 }));

        // Act
        await Client.SendMessageAsync(message: rtlMessage, recipient: RecipientNumber,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        var entry = Assert.Single(MockServer.LogEntries);
        Assert.NotNull(entry.RequestMessage);
        Assert.NotNull(entry.RequestMessage.Body);
        var request = JsonSerializer.Deserialize(entry.RequestMessage.Body, JsonBotSerializerContext.Default.SendMessageRequest);
        Assert.Equal(rtlMessage, request!.Message);
    }

    [Fact(Timeout = 5000)]
    public async Task SendMessage_VeryLongMessage_ShouldSucceed()
    {
        // Arrange
        var longMessage = new string('A', 50000);

        MockServer
            .Given(Request.Create()
                .WithPath("/v2/send")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.Created)
                .WithBodyAsJson(new { timestamp = 123456 }));

        // Act
        await Client.SendMessageAsync(message: longMessage, recipient: RecipientNumber,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        var entry = Assert.Single(MockServer.LogEntries);
        Assert.NotNull(entry.RequestMessage);
        Assert.NotNull(entry.RequestMessage.Body);
        var request = JsonSerializer.Deserialize(entry.RequestMessage.Body, JsonBotSerializerContext.Default.SendMessageRequest);
        Assert.Equal(longMessage, request!.Message);
    }

    [Fact(Timeout = 5000)]
    public async Task SendMessage_ExtremelyLongMessage_ShouldReturnNull()
    {
        // Arrange
        var tooLongMessage = new string('A', 100000);

        MockServer
            .Given(Request.Create()
                .WithPath("/v2/send")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.BadRequest)
                .WithBodyAsJson(new { error = "Message too long" }));

        // Act
        var result = await Client.SendMessageAsync(message: tooLongMessage, recipient: RecipientNumber,
                cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(result);
    }

    [Fact(Timeout = 5000)]
    public async Task SendMessage_RateLimited_ShouldReturnNull()
    {
        // Arrange
        MockServer
            .Given(Request.Create()
                .WithPath("/v2/send")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.TooManyRequests)
                .WithHeader("Retry-After", "60")
                .WithBodyAsJson(new { error = "Rate limit exceeded" }));

        // Act
        var result = await Client.SendMessageAsync(message: "Rate limit test", recipient: RecipientNumber,
                cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(result);
    }

    [Fact(Timeout = 5000)]
    public async Task RemoteDelete_ShouldSucceed()
    {
        // Arrange
        var timestamp = DateTime.UtcNow;
        var acknowledged = new Acknowledged { Timestamp = timestamp };
        var json = JsonSerializer.Serialize(acknowledged, JsonBotAPI.Options);

        MockServer
            .Given(Request.Create()
                .WithPath(path => path.Contains("/remote-delete"))
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody(json));

        // Act
        var result = await Client.RemoteDeleteAsync(RecipientNumber, timestamp, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.Single(MockServer.LogEntries));
    }
}

