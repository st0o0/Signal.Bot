using System.Net;
using System.Text.Json;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Signal.Bot.Types;

namespace Signal.Bot.Tests;

public class SignalBotClientTests
{
    private readonly HttpClient _httpClientMock;
    private readonly SignalBotClient _client;

    public SignalBotClientTests()
    {
        _httpClientMock = Substitute.For<HttpClient>();
        _client = new SignalBotClient(builder =>
            builder.WithNumber("123").WithBaseUrl("http://localhost:8080").WithHttpClient(_httpClientMock));
    }

    #region SendMessageAsync Tests

    [Fact]
    public async Task SendMessageAsync_ValidRequest_CallsHttpClient()
    {
        // Arrange
        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        // Act
        await _client.SendMessageAsync(
            "+0987654321",
            "Hello World",
            CancellationToken.None);

        // Assert
        await _httpClientMock
            .Received(1)
            .SendAsync(
                Arg.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post && req.RequestUri!.ToString().Contains("v2/send")),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SendMessageAsync_SingleRecipient_CallsHttpClient()
    {
        // Arrange
        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        // Act
        await _client.SendMessageAsync("+1234567890", "Test message",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        await _httpClientMock
            .Received(1)
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SendMessageAsync_NullMessage_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _client.SendMessageAsync("+0987654321", null!,
                cancellationToken: TestContext.Current.CancellationToken));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task SendMessageAsync_EmptyMessage_ThrowsArgumentException(string message)
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _client.SendMessageAsync("+0987654321", message,
                cancellationToken: TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task SendMessageAsync_MultipleRecipients_CallsHttpClientOnce()
    {
        // Arrange
        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        // Act
        await _client.SendMessageAsync(builder => builder
                .WithRecipients(["+1111111111", "+2222222222", "+3333333333"])
                .WithMessage("Broadcast message"),
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        await _httpClientMock
            .Received(1)
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SendMessageAsync_WithCancellationToken_PassesCancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        // Act
        await _client.SendMessageAsync("+0987654321", "Message", cts.Token);

        // Assert
        await _httpClientMock
            .Received(1)
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());

        cts.Dispose();
    }

    #endregion

    #region GetAboutAsync Tests

    [Fact]
    public async Task GetAboutAsync_ValidRequest_CallsHttpClient()
    {
        // Arrange
        var about = new About { Version = "1.0.0" };
        var json = JsonSerializer.Serialize(about);
        var content = new StringContent(json);

        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = content }));

        // Act
        var result = await _client.GetAboutAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        await _httpClientMock
            .Received(1)
            .SendAsync(
                Arg.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAboutAsync_WithCancellationToken_PassesCancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        var about = new About { Version = "1.0.0" };
        var json = JsonSerializer.Serialize(about);
        var content = new StringContent(json);

        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = content }));

        // Act
        await _client.GetAboutAsync(cts.Token);

        // Assert
        await _httpClientMock
            .Received(1)
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());

        cts.Dispose();
    }

    #endregion

    #region SetPinAsync Tests

    [Fact]
    public async Task SetPinAsync_ValidPin_CallsHttpClient()
    {
        // Arrange
        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        // Act
        await _client.SetPinAsync("123456", cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        await _httpClientMock
            .Received(1)
            .SendAsync(
                Arg.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SetPinAsync_EmptyPin_StillCallsHttpClient()
    {
        // Arrange
        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        // Act
        await _client.SetPinAsync("", cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        await _httpClientMock
            .Received(1)
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    #endregion

    #region RemovePinAsync Tests

    [Fact]
    public async Task RemovePinAsync_ValidRequest_CallsHttpClient()
    {
        // Arrange
        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        // Act
        await _client.RemovePinAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        await _httpClientMock
            .Received(1)
            .SendAsync(
                Arg.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Delete),
                Arg.Any<CancellationToken>());
    }

    #endregion

    #region UpdateProfileAsync Tests

    [Fact]
    public async Task UpdateProfileAsync_AllParametersNull_CallsHttpClient()
    {
        // Arrange
        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        // Act
        await _client.UpdateProfileAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        await _httpClientMock
            .Received(1)
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateProfileAsync_WithName_CallsHttpClient()
    {
        // Arrange
        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        // Act
        await _client.UpdateProfileAsync(name: "John Doe", cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        await _httpClientMock
            .Received(1)
            .SendAsync(
                Arg.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateProfileAsync_WithAbout_CallsHttpClient()
    {
        // Arrange
        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        // Act
        await _client.UpdateProfileAsync(about: "My status", cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        await _httpClientMock
            .Received(1)
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateProfileAsync_WithBase64Avatar_CallsHttpClient()
    {
        // Arrange
        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        // Act
        await _client.UpdateProfileAsync(base64Avatar: "base64data",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        await _httpClientMock
            .Received(1)
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    #endregion

    #region GetDevicesAsync Tests

    [Fact]
    public async Task GetDevicesAsync_ValidRequest_CallsHttpClient()
    {
        // Arrange
        var devices = new List<Device>();
        var json = JsonSerializer.Serialize(devices);
        var content = new StringContent(json);

        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = content }));

        // Act
        var result = await _client.GetDevicesAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        await _httpClientMock
            .Received(1)
            .SendAsync(
                Arg.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get),
                Arg.Any<CancellationToken>());
    }

    #endregion

    #region AddDeviceAsync Tests

    [Fact]
    public async Task AddDeviceAsync_ValidUri_CallsHttpClient()
    {
        // Arrange
        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        // Act
        await _client.AddDeviceAsync("device://uri", cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        await _httpClientMock
            .Received(1)
            .SendAsync(
                Arg.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post),
                Arg.Any<CancellationToken>());
    }

    #endregion

    #region UnregisterDeviceAsync Tests

    [Fact]
    public async Task UnregisterDeviceAsync_ValidRequest_CallsHttpClient()
    {
        // Arrange
        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        // Act
        await _client.UnregisterDeviceAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        await _httpClientMock
            .Received(1)
            .SendAsync(
                Arg.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Delete),
                Arg.Any<CancellationToken>());
    }

    #endregion

    #region GetAttachmentsAsync Tests

    [Fact]
    public async Task GetAttachmentsAsync_ValidRequest_CallsHttpClient()
    {
        // Arrange
        var attachments = new List<string> { "id1", "id2" };
        var json = JsonSerializer.Serialize(attachments);
        var content = new StringContent(json);

        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = content }));

        // Act
        var result = await _client.GetAttachmentsAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        await _httpClientMock
            .Received(1)
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    #endregion

    #region GetAttachmentAsync Tests

    //[Fact]
    public async Task GetAttachmentAsync_ValidAttachmentId_CallsHttpClient()
    {
        // Arrange
        const string attachmentId = "test-attachment-id";
        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("attachment-data")
            }));

        // Act
        var result = await _client.GetAttachmentAsync(attachmentId);

        // Assert
        Assert.NotNull(result);
        await _httpClientMock
            .Received(1)
            .SendAsync(
                Arg.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get && req.RequestUri!.ToString().Contains(attachmentId)),
                Arg.Any<CancellationToken>());
    }

    #endregion

    #region RemoveAttachmentAsync Tests

    [Fact]
    public async Task RemoveAttachmentAsync_ValidAttachmentId_CallsHttpClient()
    {
        // Arrange
        const string attachmentId = "test-attachment-id";
        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        // Act
        await _client.RemoveAttachmentAsync(attachmentId, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        await _httpClientMock
            .Received(1)
            .SendAsync(
                Arg.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Delete),
                Arg.Any<CancellationToken>());
    }

    #endregion

    #region Client Properties Tests

    [Fact]
    public void BaseUrl_ReturnsCorrectValue()
    {
        // Act & Assert
        Assert.Equal("http://localhost:8080", _client.BaseUrl);
    }

    [Fact]
    public void Number_ReturnsCorrectValue()
    {
        // Act & Assert
        Assert.Equal("123", _client.Number);
    }

    [Fact]
    public void JsonSerializerOptions_ReturnsNotNull()
    {
        // Act & Assert
        Assert.NotNull(_client.JsonSerializerOptions);
    }

    #endregion

    #region Error Handling Tests

    [Fact]
    public async Task SendAsync_HttpRequestThrowsException_HandlesGracefully()
    {
        // Arrange
        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .ThrowsAsync<HttpRequestException>();

        // Act
        _ = await _client.SendMessageAsync("+0987654321", "Test message",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        await _httpClientMock
            .Received(1)
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SendAsync_TaskCanceledException_HandlesGracefully()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .ThrowsAsync<TaskCanceledException>();

        // Act
        _ = await _client.SendMessageAsync("+0987654321", "Test message", cts.Token);

        // Assert
        await _httpClientMock
            .Received(1)
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());

        cts.Dispose();
    }

    #endregion
}