using System.Net;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using R3;
using Signal.Bot.UnitTests.Utils;

namespace Signal.Bot.UnitTests.Extensions;

public class SendMessageTests : BotTestBase
{
    private const string BotNumber = "+1234567890";
    private const string RecipientNumber = "+0987654321";

    [Fact(Timeout = 5000)]
    public async Task SendMessageAsync_WithValidMessageAndRecipient_ShouldCallHttpClient()
    {
        // Arrange
        const string message = "Hello World";
        SetupResponse();

        // Act
        await Client.SendMessageAsync(message, RecipientNumber,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        await HttpClientMock
            .Received(1)
            .SendAsync(
                Arg.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri!.ToString().Contains("v2/send")),
                Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task SendMessageAsync_WithMultipleRecipients_ShouldCallHttpClientOnce()
    {
        // Arrange
        var recipients = new[] { "+1111111111", "+2222222222", "+3333333333" };
        SetupResponse();

        // Act
        await Client.SendMessageAsync(
            builder => builder
                .WithRecipients(recipients)
                .WithMessage("Broadcast message"),
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        await HttpClientMock.Received(1).SendAsync(
            Arg.Any<HttpRequestMessage>(),
            Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task SendMessageAsync_WithNullMessage_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            Client.SendMessageAsync(null!, RecipientNumber,
                cancellationToken: TestContext.Current.CancellationToken));
    }

    [Theory(Timeout = 5000)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    public async Task SendMessageAsync_WithEmptyMessage_ShouldThrowArgumentException(string message)
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            Client.SendMessageAsync(message, RecipientNumber,
                cancellationToken: TestContext.Current.CancellationToken));
    }

    [Fact(Timeout = 5000)]
    public async Task SendMessageAsync_WithUnicodeCharacters_ShouldCallHttpClient()
    {
        // Arrange
        const string message = "Hello! 你好 🎉 @#$%^&*()";
        SetupResponse();

        // Act
        await Client.SendMessageAsync(message, RecipientNumber,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        await HttpClientMock.Received(1).SendAsync(
            Arg.Any<HttpRequestMessage>(),
            Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task SendMessageAsync_WithVeryLongMessage_ShouldCallHttpClient()
    {
        // Arrange
        var longMessage = new string('A', 5000);
        SetupResponse();

        // Act
        await Client.SendMessageAsync(longMessage, RecipientNumber,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        await HttpClientMock.Received(1).SendAsync(
            Arg.Any<HttpRequestMessage>(),
            Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task SendMessageAsync_WithCancellationToken_ShouldPassTokenToHttpClient()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        SetupResponse();

        // Act
        await Client.SendMessageAsync("Test", RecipientNumber, cts.Token);

        // Assert
        await HttpClientMock.Received(1).SendAsync(
            Arg.Any<HttpRequestMessage>(),
            Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task SendMessageAsync_WithAlreadyCancelledToken_ShouldHandleGracefully()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        SetupResponse();

        // Act
        var result = await Client.SendMessageAsync("Test", RecipientNumber, cts.Token);

        // Assert
        Assert.Null(result);
    }

    [Theory(Timeout = 5000)]
    [InlineData(HttpStatusCode.Created)]
    [InlineData(HttpStatusCode.Accepted)]
    [InlineData(HttpStatusCode.NoContent)]
    public async Task SendMessageAsync_WithSuccessStatusCodes_ShouldSucceed(HttpStatusCode statusCode)
    {
        // Arrange
        SetupResponse(statusCode);

        // Act
        await Client.SendMessageAsync("Test", RecipientNumber,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        await HttpClientMock.Received(1).SendAsync(
            Arg.Any<HttpRequestMessage>(),
            Arg.Any<CancellationToken>());
    }

    [Theory(Timeout = 5000)]
    [InlineData("+1-234-567-8900")]
    [InlineData("+49 170 1234567")]
    [InlineData("+33123456789")]
    public async Task SendMessageAsync_WithDifferentPhoneNumberFormats_ShouldCallHttpClient(string phoneNumber)
    {
        // Arrange
        SetupResponse();

        // Act
        await Client.SendMessageAsync("Test", phoneNumber,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        await HttpClientMock.Received(1).SendAsync(
            Arg.Any<HttpRequestMessage>(),
            Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task SendMessageAsync_WithHttpException_HandlesGracefully()
    {
        HttpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new HttpRequestException());

        var exceptions = 0;
        using var t = Client.OnException.Subscribe(x =>
        {
            if (x is HttpRequestException)
            {
                exceptions++;
            }
        });
        await Client.SendMessageAsync("+1234567890", "Test message",
            cancellationToken: TestContext.Current.CancellationToken);

        await HttpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
        Assert.Equal(1, exceptions);
    }

    [Fact(Timeout = 5000)]
    public async Task SendMessageAsync_MultipleCallsInParallel_AllSucceed()
    {
        SetupResponse();

        var tasks = Enumerable.Range(1, 5)
            .Select(i => Client.SendMessageAsync($"+123456789{i}", $"Message {i}"))
            .ToList();

        await Task.WhenAll(tasks);

        await HttpClientMock.Received(5).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task SendMessageAsync_WithSpecialCharactersInMessage_CallsHttpClient()
    {
        const string message = "Hello! 你好 🎉 @#$%^&*()";
        SetupResponse();

        await Client.SendMessageAsync("+1234567890", message,
            cancellationToken: TestContext.Current.CancellationToken);

        await HttpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task SendMessageAsync_WithVeryLongMessage_CallsHttpClient()
    {
        var message = new string('a', 5000);
        SetupResponse();

        await Client.SendMessageAsync("+1234567890", message,
            cancellationToken: TestContext.Current.CancellationToken);

        await HttpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task SendMessageAsync_WithPhoneNumberVariants_CallsHttpClient()
    {
        SetupResponse();

        await Client.SendMessageAsync("+1-234-567-8900", "Message",
            cancellationToken: TestContext.Current.CancellationToken);

        await HttpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }
}
