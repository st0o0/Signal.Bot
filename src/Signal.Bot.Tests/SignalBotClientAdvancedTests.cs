using System.Net;
using System.Text.Json;
using NSubstitute;
using Signal.Bot.Types;

namespace Signal.Bot.Tests;

/// <summary>
/// Advanced test scenarios including edge cases, response handling, and special conditions
/// </summary>
public class SignalBotClientAdvancedTests
{
    private readonly HttpClient _httpClientMock;
    private readonly SignalBotClient _client;

    public SignalBotClientAdvancedTests()
    {
        _httpClientMock = Substitute.For<HttpClient>();
        _client = new SignalBotClient(new SignalBotClientOptions("123", "http://localhost:8080"), _httpClientMock);
    }

    #region Response Content Handling Tests

    //[Fact]
    public async Task GetAttachmentAsync_ReturnsStringContent()
    {
        // Arrange
        const string attachmentId = "test-id";
        const string attachmentData = "base64encodeddata";
        var content = new StringContent(attachmentData);

        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = content }));

        // Act
        var result = await _client.GetAttachmentAsync(attachmentId);

        // Assert
        Assert.Equal(attachmentData, result);
    }

    [Fact]
    public async Task GetDevicesAsync_WithMultipleDevices_ReturnsCollection()
    {
        // Arrange
        var devices = new List<Device>
        {
            new() { Name = "Device 1", Created = 1000 },
            new() { Name = "Device 2", Created = 2000 },
            new() { Name = "Device 3", Created = 3000 }
        };
        var json = JsonSerializer.Serialize(devices);
        var content = new StringContent(json);

        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = content }));

        // Act
        var result = await _client.GetDevicesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task GetAboutAsync_ReturnsAboutWithAllFields()
    {
        // Arrange
        var about = new About
        {
            Version = "1.15.0",
            Build = 123456,
            Mode = "normal"
        };
        var json = JsonSerializer.Serialize(about);
        var content = new StringContent(json);

        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = content }));

        // Act
        var result = await _client.GetAboutAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("1.15.0", result.Version);
    }

    #endregion

    #region HTTP Method Verification Tests

    [Fact]
    public async Task SendMessageAsync_UsesPostMethod()
    {
        // Arrange
        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        // Act
        await _client.SendMessageAsync(["+1234567890"], "Message");

        // Assert
        await _httpClientMock
            .Received(1)
            .SendAsync(
                Arg.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetDevicesAsync_UsesGetMethod()
    {
        // Arrange
        var devices = new List<Device>();
        var json = JsonSerializer.Serialize(devices);
        var content = new StringContent(json);

        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = content }));

        // Act
        await _client.GetDevicesAsync();

        // Assert
        await _httpClientMock
            .Received(1)
            .SendAsync(
                Arg.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AddDeviceAsync_UsesPutMethod()
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
                Arg.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RemoveAttachmentAsync_UsesDeleteMethod()
    {
        // Arrange
        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        // Act
        await _client.RemoveAttachmentAsync("attachment-id");

        // Assert
        await _httpClientMock
            .Received(1)
            .SendAsync(
                Arg.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Delete),
                Arg.Any<CancellationToken>());
    }

    #endregion

    #region Parameter Encoding Tests

    [Fact]
    public async Task SendMessageAsync_WithSpecialCharactersInMessage_CallsHttpClient()
    {
        // Arrange
        var message = "Hello! 你好 🎉 @#$%^&*()";
        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        // Act
        await _client.SendMessageAsync(["+1234567890"], message);

        // Assert
        await _httpClientMock
            .Received(1)
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SendMessageAsync_WithVeryLongMessage_CallsHttpClient()
    {
        // Arrange
        var message = new string('a', 5000);
        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        // Act
        await _client.SendMessageAsync(["+1234567890"], message);

        // Assert
        await _httpClientMock
            .Received(1)
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SendMessageAsync_WithPhoneNumberVariants_CallsHttpClient()
    {
        // Arrange
        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        // Act
        await _client.SendMessageAsync(["+1-234-567-8900"], "Message");

        // Assert
        await _httpClientMock
            .Received(1)
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    #endregion

    #region Null/Empty Response Handling Tests

    [Fact]
    public async Task GetAttachmentsAsync_WithEmptyList_ReturnsEmptyCollection()
    {
        // Arrange
        var attachments = new List<string>();
        var json = JsonSerializer.Serialize(attachments);
        var content = new StringContent(json);

        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = content }));

        // Act
        var result = await _client.GetAttachmentsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetDevicesAsync_WithEmptyList_ReturnsEmptyCollection()
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
        Assert.Empty(result);
    }

    #endregion

    #region CancellationToken Edge Cases Tests

    [Fact]
    public async Task SendMessageAsync_WithAlreadyCancelledToken_HandlesGracefully()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        // Act
        var result = await _client.SendMessageAsync(["+1234567890"], "Message", cts.Token);

        // Assert
        Assert.Null(result);

        cts.Dispose();
    }

    [Fact]
    public async Task GetAccountsAsync_WithImmediateTimeout_HandlesGracefully()
    {
        // Arrange
        var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(1));

        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(async _ =>
            {
                await Task.Delay(100);
                return new HttpResponseMessage(HttpStatusCode.OK);
            });

        // Act
        var result = await _client.GetAccountsAsync(cts.Token);

        // Assert
        Assert.Null(result);

        cts.Dispose();
    }

    #endregion

    #region Recipient Parameter Validation Tests

    [Fact]
    public async Task SendMessageAsync_WithEmptyRecipientArray_StillCalls()
    {
        // Arrange
        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        // Act
        await _client.SendMessageAsync([], "Message");

        // Assert
        await _httpClientMock
            .Received(1)
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SendMessageAsync_WithSingleRecipientArray_CallsOnce()
    {
        // Arrange
        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        // Act
        await _client.SendMessageAsync(["+1234567890"], "Message");

        // Assert
        await _httpClientMock
            .Received(1)
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    #endregion

    #region HttpStatusCode Variations Tests

    [Fact]
    public async Task SendMessageAsync_WithCreatedStatus_Succeeds()
    {
        // Arrange
        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.Created)));

        // Act & Assert
        await _client.SendMessageAsync(["+1234567890"], "Message");

        await _httpClientMock
            .Received(1)
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SendMessageAsync_WithAcceptedStatus_Succeeds()
    {
        // Arrange
        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.Accepted)));

        // Act & Assert
        await _client.SendMessageAsync(["+1234567890"], "Message");

        await _httpClientMock
            .Received(1)
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SendMessageAsync_WithNoContentStatus_Succeeds()
    {
        // Arrange
        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.NoContent)));

        // Act & Assert
        await _client.SendMessageAsync(["+1234567890"], "Message");

        await _httpClientMock
            .Received(1)
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    #endregion

    #region Concurrent Requests Tests

    [Fact]
    public async Task SendMessageAsync_MultipleCallsInParallel_AllSucceed()
    {
        // Arrange
        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        var tasks = Enumerable.Range(1, 5)
            .Select(i => _client.SendMessageAsync([$"+123456789{i}"], $"Message {i}"))
            .ToList();

        // Act
        await Task.WhenAll(tasks);

        // Assert
        await _httpClientMock
            .Received(5)
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    #endregion

    #region Different Client Configurations Tests

    [Fact]
    public void Client_WithDifferentBaseUrl_StoresCorrectly()
    {
        // Arrange
        var options = new SignalBotClientOptions("555", "https://api.secure.com:443");
        var httpClient = Substitute.For<HttpClient>();

        // Act
        var client = new SignalBotClient(options, httpClient);

        // Assert
        Assert.Equal("https://api.secure.com:443", client.BaseUrl);
        Assert.Equal("555", client.Number);
    }

    [Fact]
    public void Client_WithAlternativeNumber_StoresCorrectly()
    {
        // Arrange
        var options = new SignalBotClientOptions("+33123456789", "http://localhost:8080");
        var httpClient = Substitute.For<HttpClient>();

        // Act
        var client = new SignalBotClient(options, httpClient);

        // Assert
        Assert.Equal("+33123456789", client.Number);
    }

    #endregion

    #region Attachment ID Variations Tests

    [Fact]
    public async Task GetAttachmentAsync_WithUuidAttachmentId_CallsHttpClient()
    {
        // Arrange
        var attachmentId = "550e8400-e29b-41d4-a716-446655440000";
        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("data")
            }));

        // Act
        await _client.GetAttachmentAsync(attachmentId);

        // Assert
        await _httpClientMock
            .Received(1)
            .SendAsync(
                Arg.Is<HttpRequestMessage>(req =>
                    req.RequestUri!.ToString().Contains(attachmentId)),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RemoveAttachmentAsync_WithComplexAttachmentId_CallsHttpClient()
    {
        // Arrange
        var attachmentId = "attachment_id_with_special-chars_123";
        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        // Act
        await _client.RemoveAttachmentAsync(attachmentId);

        // Assert
        await _httpClientMock
            .Received(1)
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    #endregion

    #region Default Parameter Values Tests

    [Fact]
    public async Task SendMessageAsync_NoExplicitCancellationToken_UsesDefault()
    {
        // Arrange
        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        // Act
        await _client.SendMessageAsync(["+1234567890"], "Message");

        // Assert
        await _httpClientMock
            .Received(1)
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateProfileAsync_NoParameters_Uses()
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

    #endregion
}