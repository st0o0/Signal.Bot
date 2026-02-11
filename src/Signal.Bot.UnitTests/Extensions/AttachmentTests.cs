using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using NSubstitute;
using R3;
using Signal.Bot.UnitTests.Utils;

namespace Signal.Bot.UnitTests.Extensions;

public class AttachmentTests : BotTestBase
{
    [Fact(Timeout = 5000)]
    public async Task GetAttachmentAsync_ReturnsBytes()
    {
        // Arrange
        const string attachmentId = "test-id";
        var attachmentData = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }; // JPEG header

        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent(attachmentData)
        };
        responseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

        HttpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(responseMessage));

        // Act
        var result =
            await Client.GetAttachmentAsync(attachmentId, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(attachmentData, result);
    }

    [Fact(Timeout = 5000)]
    public async Task GetAttachmentAsync_AsStream_ReturnsStream()
    {
        // Arrange
        const string attachmentId = "stream-test";
        var attachmentData = new byte[] { 1, 2, 3, 4, 5 };

        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent(attachmentData)
        };

        HttpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(responseMessage));

        // Act
        await using var stream = await Client.GetAttachmentStreamAsync(attachmentId,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream, TestContext.Current.CancellationToken);
        Assert.Equal(attachmentData, memoryStream.ToArray());
    }


    [Fact(Timeout = 5000)]
    public async Task GetAttachmentAsync_WithCancellationToken_PassesTokenThrough()
    {
        // Arrange
        const string attachmentId = "cancel-test";
        var attachmentData = new byte[] { 1, 2, 3 };
        using var cts = new CancellationTokenSource();

        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent(attachmentData)
        };

        HttpRequestMessage? capturedRequest = null;
        var capturedToken = CancellationToken.None;

        HttpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                capturedRequest = callInfo.ArgAt<HttpRequestMessage>(0);
                capturedToken = callInfo.ArgAt<CancellationToken>(1);
                return Task.FromResult(responseMessage);
            });

        // Act
        await Client.GetAttachmentAsync(attachmentId, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(capturedRequest);
        Assert.Equal(cts.Token.IsCancellationRequested, capturedToken.IsCancellationRequested);
    }

    [Fact(Timeout = 5000)]
    public async Task GetAttachmentAsync_WithInvalidId_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            Client.GetAttachmentAsync(null!, cancellationToken: TestContext.Current.CancellationToken));

        await Assert.ThrowsAsync<ArgumentException>(() =>
            Client.GetAttachmentAsync("", cancellationToken: TestContext.Current.CancellationToken));

        await Assert.ThrowsAsync<ArgumentException>(() =>
            Client.GetAttachmentAsync("       ", cancellationToken: TestContext.Current.CancellationToken));
    }

    [Fact(Timeout = 5000)]
    public async Task GetAttachmentAsync_WhenServerReturnsError_ThrowsException()
    {
        // Arrange
        const string attachmentId = "error-test";

        var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent(JsonSerializer.Serialize(new { error = "Attachment not found" }))
        };

        HttpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(responseMessage));

        Exception catchedException = null!;
        Client.OnException.Subscribe(ex => catchedException = ex);
        // Act 
        await Assert.ThrowsAsync<NullReferenceException>(() =>
            Client.GetAttachmentAsync(attachmentId, cancellationToken: TestContext.Current.CancellationToken));

        //Assert
        Assert.IsType<HttpRequestException>(catchedException);
    }

    [Fact(Timeout = 5000)]
    public async Task GetAttachmentAsync_WhenServerReturns400_ThrowsException()
    {
        // Arrange
        const string attachmentId = "bad-request-test";
        const string message = "Invalid attachment request";

        var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent(JsonSerializer.Serialize(new Types.ErrorResponse { Message = message }))
        };

        HttpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(responseMessage));

        Exception catchedException = null!;
        Client.OnException.Subscribe(ex => catchedException = ex);

        // Act 
        await Assert.ThrowsAsync<NullReferenceException>(() =>
            Client.GetAttachmentAsync(attachmentId, cancellationToken: TestContext.Current.CancellationToken));

        // Assert
        Assert.IsType<HttpRequestException>(catchedException);
        Assert.Equal(message, catchedException.Message);
    }

    [Fact(Timeout = 5000)]
    public async Task GetAttachmentAsync_LargeFile_HandlesCorrectly()
    {
        // Arrange
        const string attachmentId = "large-file";
        var largeData = new byte[100 * 1024 * 1024]; // 100MB
        new Random().NextBytes(largeData);

        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent(largeData)
        };
        responseMessage.Content.Headers.ContentLength = largeData.Length;

        HttpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(responseMessage));

        // Act
        var result =
            await Client.GetAttachmentAsync(attachmentId, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(largeData.Length, result.Length);
        Assert.Equal(largeData, result);
    }

    [Theory(Timeout = 5000)]
    [InlineData("image/jpeg", new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 })]
    [InlineData("image/png", new byte[] { 0x89, 0x50, 0x4E, 0x47 })]
    [InlineData("application/pdf", new byte[] { 0x25, 0x50, 0x44, 0x46 })]
    public async Task GetAttachmentAsync_DifferentContentTypes_ReturnsCorrectData(
        string contentType,
        byte[] expectedHeader)
    {
        // Arrange
        var attachmentId = $"test-{contentType.Replace("/", "-")}";
        var fullData = new byte[expectedHeader.Length + 100];
        Array.Copy(expectedHeader, fullData, expectedHeader.Length);

        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent(fullData)
        };
        responseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

        HttpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(responseMessage));

        // Act
        var result =
            await Client.GetAttachmentAsync(attachmentId, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(fullData.Length, result.Length);
        Assert.Equal(expectedHeader, result.Take(expectedHeader.Length).ToArray());
    }

    [Fact(Timeout = 5000)]
    public async Task GetAttachmentAsync_DisposesResponseProperly()
    {
        // Arrange
        const string attachmentId = "dispose-test";
        var attachmentData = new byte[] { 1, 2, 3 };

        var content = new ByteArrayContent(attachmentData);
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = content
        };

        HttpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(responseMessage));

        // Act
        var result =
            await Client.GetAttachmentAsync(attachmentId, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
    }

    [Fact(Timeout = 5000)]
    public async Task GetAttachmentAsync_WithUuidAttachmentId_CallsHttpClient()
    {
        const string attachmentId = "550e8400-e29b-41d4-a716-446655440000";
        HttpClientMock.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("data")
            }));

        await Client.GetAttachmentAsync(attachmentId, cancellationToken: TestContext.Current.CancellationToken);

        await HttpClientMock.Received(1).SendAsync(
            Arg.Is<HttpRequestMessage>(req => req.RequestUri!.ToString().Contains(attachmentId)),
            Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task RemoveAttachmentAsync_WithComplexAttachmentId_CallsHttpClient()
    {
        const string attachmentId = "attachment_id_with_special-chars_123";
        HttpClientMock.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        await Client.RemoveAttachmentAsync(attachmentId, cancellationToken: TestContext.Current.CancellationToken);

        await HttpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task RemoveAttachmentAsync_ValidAttachmentId_CallsHttpClient()
    {
        // Arrange
        const string attachmentId = "test-attachment-id";
        SetupResponse();

        // Act
        await Client.RemoveAttachmentAsync(attachmentId, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        await HttpClientMock
            .Received(1)
            .SendAsync(
                Arg.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Delete),
                Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task GetAttachmentsAsync_ValidRequest_CallsHttpClient()
    {
        // Arrange
        var attachments = new List<string> { "id1", "id2" };
        var json = JsonSerializer.Serialize(attachments);

        SetupJsonResponse(json);

        // Act
        var result = await Client.GetAttachmentsAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        await HttpClientMock
            .Received(1)
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task GetAttachmentAsync_ValidAttachmentId_CallsHttpClient()
    {
        // Arrange
        const string attachmentId = "test-attachment-id";
        var testBytes = new byte[]
        {
            0x00, 0x01, 0x02, 0x10, 0x20,
            0x7F, 0x80, 0xAA, 0xFE, 0xFF
        };

        HttpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(testBytes)
            }));

        // Act
        var result =
            await Client.GetAttachmentAsync(attachmentId, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        await HttpClientMock
            .Received(1)
            .SendAsync(
                Arg.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get && req.RequestUri!.ToString().Contains(attachmentId)),
                Arg.Any<CancellationToken>());
    }
}
