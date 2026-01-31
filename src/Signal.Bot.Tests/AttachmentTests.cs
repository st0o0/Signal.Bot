using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using NSubstitute;

namespace Signal.Bot.Tests;

public class AttachmentTests
{
    private readonly HttpClient _httpClientMock;
    private readonly SignalBotClient _client;

    public AttachmentTests()
    {
        _httpClientMock = Substitute.For<HttpClient>();
        _client = new SignalBotClient(builder =>
            builder.WithNumber("123").WithBaseUrl("http://localhost:8080").WithHttpClient(_httpClientMock));
    }

    [Fact]
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

        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(responseMessage));

        // Act
        var result =
            await _client.GetAttachmentAsync(attachmentId, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(attachmentData, result);
    }

    [Fact]
    public async Task GetAttachmentAsync_AsStream_ReturnsStream()
    {
        // Arrange
        const string attachmentId = "stream-test";
        var attachmentData = new byte[] { 1, 2, 3, 4, 5 };

        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent(attachmentData)
        };

        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(responseMessage));

        // Act
        await using var stream = await _client.GetAttachmentStreamAsync(attachmentId,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream, TestContext.Current.CancellationToken);
        Assert.Equal(attachmentData, memoryStream.ToArray());
    }


    [Fact]
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
        CancellationToken capturedToken = default;

        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                capturedRequest = callInfo.ArgAt<HttpRequestMessage>(0);
                capturedToken = callInfo.ArgAt<CancellationToken>(1);
                return Task.FromResult(responseMessage);
            });

        // Act
        await _client.GetAttachmentAsync(attachmentId, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(capturedRequest);
        Assert.Equal(cts.Token.IsCancellationRequested, capturedToken.IsCancellationRequested);
    }

    [Fact]
    public async Task GetAttachmentAsync_WithInvalidId_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _client.GetAttachmentAsync(null!, cancellationToken: TestContext.Current.CancellationToken));

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _client.GetAttachmentAsync("", cancellationToken: TestContext.Current.CancellationToken));

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _client.GetAttachmentAsync("       ", cancellationToken: TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task GetAttachmentAsync_WhenServerReturnsError_ThrowsException()
    {
        // Arrange
        const string attachmentId = "error-test";

        var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent(JsonSerializer.Serialize(new { error = "Attachment not found" }))
        };

        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(responseMessage));

        Exception catchedException = null!;
        _client.OnException.Subscribe(ex => catchedException = ex);
        // Act 
        await Assert.ThrowsAsync<NullReferenceException>(() =>
            _client.GetAttachmentAsync(attachmentId, cancellationToken: TestContext.Current.CancellationToken));

        //Assert
        Assert.IsType<HttpRequestException>(catchedException);
    }

    [Fact]
    public async Task GetAttachmentAsync_LargeFile_HandlesCorrectly()
    {
        // Arrange
        const string attachmentId = "large-file";
        var largeData = new byte[10 * 1024 * 1024]; // 10MB
        new Random().NextBytes(largeData);

        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent(largeData)
        };
        responseMessage.Content.Headers.ContentLength = largeData.Length;

        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(responseMessage));

        // Act
        var result =
            await _client.GetAttachmentAsync(attachmentId, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(largeData.Length, result.Length);
        Assert.Equal(largeData, result);
    }
    
    [Theory]
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

        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(responseMessage));

        // Act
        var result =
            await _client.GetAttachmentAsync(attachmentId, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(fullData.Length, result.Length);
        Assert.Equal(expectedHeader, result.Take(expectedHeader.Length).ToArray());
    }

    [Fact]
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

        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(responseMessage));

        // Act
        var result =
            await _client.GetAttachmentAsync(attachmentId, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
    }
}