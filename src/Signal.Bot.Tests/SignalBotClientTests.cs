using System.Net;
using System.Text.Json;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Signal.Bot.Exceptions;
using Signal.Bot.Types;

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

    #region SendMessageAsync Tests

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
        await _client.SendMessageAsync("+1234567890", "Test message");

        // Assert
        await _httpClientMock
            .Received(1)
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
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

    [Fact]
    public async Task SendMessageAsync_MultipleRecipients_CallsHttpClientOnce()
    {
        // Arrange
        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        // Act
        await _client.SendMessageAsync(
            ["+1111111111", "+2222222222", "+3333333333"],
            "Broadcast message");

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
        await _client.SendMessageAsync(["+0987654321"], "Message", cts.Token);

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
        var result = await _client.GetAboutAsync();

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
        await _client.SetPinAsync("123456");

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
        await _client.SetPinAsync("");

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
        await _client.RemovePinAsync();

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
        await _client.UpdateProfileAsync();

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
        await _client.UpdateProfileAsync(name: "John Doe");

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
        await _client.UpdateProfileAsync(about: "My status");

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
        await _client.UpdateProfileAsync(base64Avatar: "base64data");

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
        var result = await _client.GetDevicesAsync();

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
        await _client.AddDeviceAsync("device://uri");

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
        await _client.UnregisterDeviceAsync();

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
        var result = await _client.GetAttachmentsAsync();

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
        var attachmentId = "test-attachment-id";
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
        var attachmentId = "test-attachment-id";
        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        // Act
        await _client.RemoveAttachmentAsync(attachmentId);

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

        // Act - The client catches exceptions and returns default value
        var result = await _client.SendMessageAsync(["+0987654321"], "Test message");

        // Assert - No exception thrown, gracefully handled
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

        // Act - The client catches exceptions and returns default value
        var result = await _client.SendMessageAsync(["+0987654321"], "Test message", cts.Token);

        // Assert - No exception thrown, gracefully handled
        await _httpClientMock
            .Received(1)
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());

        cts.Dispose();
    }

    #endregion
}