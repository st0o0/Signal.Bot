using System.Net;
using System.Text.Json;
using JetBrains.Annotations;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Signal.Bot.Internal;
using Signal.Bot.Requests;
using Signal.Bot.Types;

namespace Signal.Bot.Tests;

[TestSubject(typeof(SignalBotClient))]
public class SignalBotClientCompleteTests
{
    private readonly HttpClient _httpClientMock;
    private readonly SignalBotClient _client;
    private readonly ISignalBotClient _clientInterfaceMock;

    public SignalBotClientCompleteTests()
    {
        _httpClientMock = Substitute.For<HttpClient>();
        _clientInterfaceMock = Substitute.For<ISignalBotClient>();
        _clientInterfaceMock.Number.Returns("123");
        _client = new SignalBotClient(new SignalBotClientOptions("123", "http://localhost:8080"), _httpClientMock);
    }

    #region Response Content Handling Tests

    //[Fact]
    public async Task GetAttachmentAsync_ReturnsStringContent()
    {
        const string attachmentId = "test-id";
        const string attachmentData = "base64encodeddata";
        var content = new StringContent(attachmentData);

        _httpClientMock.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = content }));

        var result = await _client.GetAttachmentAsync(attachmentId);

        Assert.Equal(attachmentData, result);
    }

    [Fact]
    public async Task GetDevicesAsync_WithMultipleDevices_ReturnsCollection()
    {
        var devices = new List<Device>
        {
            new() { Name = "Device 1", Created = 1000 },
            new() { Name = "Device 2", Created = 2000 },
            new() { Name = "Device 3", Created = 3000 }
        };
        var json = JsonSerializer.Serialize(devices);
        var content = new StringContent(json);

        _httpClientMock.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = content }));

        var result = await _client.GetDevicesAsync();

        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task GetAboutAsync_ReturnsAboutWithAllFields()
    {
        var about = new About { Version = "1.15.0", Build = 123456, Mode = "normal" };
        var json = JsonSerializer.Serialize(about);
        var content = new StringContent(json);

        _httpClientMock.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = content }));

        var result = await _client.GetAboutAsync();

        Assert.NotNull(result);
        Assert.Equal("1.15.0", result.Version);
    }

    [Fact]
    public async Task GetAccountsAsync_ValidRequest_ReturnsAccounts()
    {
        var accounts = new List<string> { "account1", "account2" };
        var json = JsonSerializer.Serialize(accounts);
        var content = new StringContent(json);

        _httpClientMock.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = content }));

        var result = await _client.GetAccountsAsync();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetGroupsAsync_ValidRequest_ReturnsGroups()
    {
        var groups = new List<Group>();
        var json = JsonSerializer.Serialize(groups);
        var content = new StringContent(json);

        _httpClientMock.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = content }));

        var result = await _client.GetGroupsAsync();

        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetAttachmentsAsync_WithEmptyList_ReturnsEmptyCollection()
    {
        var json = JsonSerializer.Serialize(new List<string>());
        var content = new StringContent(json);

        _httpClientMock.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = content }));

        var result = await _client.GetAttachmentsAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetDevicesAsync_WithEmptyList_ReturnsEmptyCollection()
    {
        var devices = new List<Device>();
        var json = JsonSerializer.Serialize(devices);
        var content = new StringContent(json);

        _httpClientMock.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = content }));

        var result = await _client.GetDevicesAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    #endregion

    #region HTTP Method Verification Tests

    [Fact]
    public async Task SendMessageAsync_UsesPostMethod()
    {
        _httpClientMock.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        await _client.SendMessageAsync(["+1234567890"], "Message");

        await _httpClientMock.Received(1).SendAsync(
            Arg.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetDevicesAsync_UsesGetMethod()
    {
        _httpClientMock.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        await _client.GetDevicesAsync();

        await _httpClientMock.Received(1).SendAsync(
            Arg.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AddDeviceAsync_UsesPostMethod()
    {
        _httpClientMock.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        await _client.AddDeviceAsync("device://uri");

        await _httpClientMock.Received(1).SendAsync(
            Arg.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RemoveAttachmentAsync_UsesDeleteMethod()
    {
        _httpClientMock.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        await _client.RemoveAttachmentAsync("attachment-id");

        await _httpClientMock.Received(1).SendAsync(
            Arg.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Delete),
            Arg.Any<CancellationToken>());
    }

    #endregion

    #region Parameter Encoding Tests

    [Fact]
    public async Task SendMessageAsync_WithSpecialCharactersInMessage_CallsHttpClient()
    {
        var message = "Hello! 你好 🎉 @#$%^&*()";
        _httpClientMock.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        await _client.SendMessageAsync(["+1234567890"], message);

        await _httpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SendMessageAsync_WithVeryLongMessage_CallsHttpClient()
    {
        var message = new string('a', 5000);
        _httpClientMock.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        await _client.SendMessageAsync(["+1234567890"], message);

        await _httpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SendMessageAsync_WithPhoneNumberVariants_CallsHttpClient()
    {
        _httpClientMock.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        await _client.SendMessageAsync(["+1-234-567-8900"], "Message");

        await _httpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    #endregion

    #region CancellationToken Edge Cases Tests

    [Fact]
    public async Task SendMessageAsync_WithAlreadyCancelledToken_HandlesGracefully()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        _httpClientMock.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        var result = await _client.SendMessageAsync(["+1234567890"], "Message", cts.Token);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAccountsAsync_WithImmediateTimeout_HandlesGracefully()
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(1));

        _httpClientMock.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(async _ =>
            {
                await Task.Delay(100);
                return new HttpResponseMessage(HttpStatusCode.OK);
            });

        var result = await _client.GetAccountsAsync(cts.Token);

        Assert.Null(result);
    }

    #endregion

    #region Recipient Parameter Validation Tests

    [Fact]
    public async Task SendMessageAsync_WithEmptyRecipientArray_StillCalls()
    {
        _httpClientMock.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        await _client.SendMessageAsync([], "Message");

        await _httpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SendMessageAsync_WithSingleRecipientArray_CallsOnce()
    {
        _httpClientMock.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        await _client.SendMessageAsync(["+1234567890"], "Message");

        await _httpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    #endregion

    #region HttpStatusCode Variations Tests

    [Fact]
    public async Task SendMessageAsync_WithCreatedStatus_Succeeds()
    {
        _httpClientMock.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.Created)));

        await _client.SendMessageAsync(["+1234567890"], "Message");

        await _httpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SendMessageAsync_WithAcceptedStatus_Succeeds()
    {
        _httpClientMock.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.Accepted)));

        await _client.SendMessageAsync(["+1234567890"], "Message");

        await _httpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SendMessageAsync_WithNoContentStatus_Succeeds()
    {
        _httpClientMock.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.NoContent)));

        await _client.SendMessageAsync(["+1234567890"], "Message");

        await _httpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    #endregion

    #region Concurrent Requests Tests

    [Fact]
    public async Task SendMessageAsync_MultipleCallsInParallel_AllSucceed()
    {
        _httpClientMock.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        var tasks = Enumerable.Range(1, 5)
            .Select(i => _client.SendMessageAsync([$"+123456789{i}"], $"Message {i}"))
            .ToList();

        await Task.WhenAll(tasks);

        await _httpClientMock.Received(5).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    #endregion

    #region Different Client Configurations Tests

    [Fact]
    public void Client_WithDifferentBaseUrl_StoresCorrectly()
    {
        var options = new SignalBotClientOptions("555", "https://api.secure.com:443");
        var httpClient = Substitute.For<HttpClient>();
        var client = new SignalBotClient(options, httpClient);

        Assert.Equal("https://api.secure.com:443", client.BaseUrl);
        Assert.Equal("555", client.Number);
    }

    [Fact]
    public void Client_WithAlternativeNumber_StoresCorrectly()
    {
        var options = new SignalBotClientOptions("+33123456789", "http://localhost:8080");
        var httpClient = Substitute.For<HttpClient>();
        var client = new SignalBotClient(options, httpClient);

        Assert.Equal("+33123456789", client.Number);
    }

    #endregion

    #region Attachment ID Variations Tests

    [Fact]
    public async Task GetAttachmentAsync_WithUuidAttachmentId_CallsHttpClient()
    {
        var attachmentId = "550e8400-e29b-41d4-a716-446655440000";
        _httpClientMock.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("data")
            }));

        await _client.GetAttachmentAsync(attachmentId);

        await _httpClientMock.Received(1).SendAsync(
            Arg.Is<HttpRequestMessage>(req => req.RequestUri!.ToString().Contains(attachmentId)),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RemoveAttachmentAsync_WithComplexAttachmentId_CallsHttpClient()
    {
        var attachmentId = "attachment_id_with_special-chars_123";
        _httpClientMock.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        await _client.RemoveAttachmentAsync(attachmentId);

        await _httpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    #endregion

    #region RegisterNumberAsync Tests

    [Fact]
    public async Task RegisterNumberAsync_WithoutParameters_CallsHttpClient()
    {
        _httpClientMock.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        await _client.RegisterNumberAsync();

        await _httpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RegisterNumberAsync_WithCaptcha_CallsHttpClient()
    {
        _httpClientMock.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        await _client.RegisterNumberAsync(captcha: "captcha-token");

        await _httpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RegisterNumberAsync_WithVoiceOption_CallsHttpClient()
    {
        _httpClientMock.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        await _client.RegisterNumberAsync(useVoice: true);

        await _httpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    #endregion

    #region VerifyNumberAsync Tests

    [Fact]
    public async Task VerifyNumberAsync_ValidToken_ReturnsVerificationResult()
    {
        var token = "verification-token";
        var expectedResult = "verified-token";
        var json = JsonSerializer.Serialize(expectedResult);
        var content = new StringContent(json);

        _httpClientMock.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = content }));

        var result = await _client.VerifyNumberAsync(token);

        Assert.Equal("verified-token", result);
    }

    [Fact]
    public async Task VerifyNumberAsync_WithPin_CallsHttpClient()
    {
        var token = "verification-token";
        var pin = "123456";
        _httpClientMock.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        await _client.VerifyNumberAsync(token, pin);

        await _httpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    #endregion

    #region SetTypingIndicatorAsync Tests

    [Fact]
    public async Task SetTypingIndicatorAsync_WithRecipient_CallsHttpClient()
    {
        _httpClientMock.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        await _client.SetTypingIndicatorAsync(recipient: "+1234567890");

        await _httpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SetTypingIndicatorAsync_WithGroupId_CallsHttpClient()
    {
        _httpClientMock.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        await _client.SetTypingIndicatorAsync(groupId: "group-id");

        await _httpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SetTypingIndicatorAsync_IsTypingFalse_CallsHttpClient()
    {
        _httpClientMock.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        await _client.SetTypingIndicatorAsync(recipient: "+1234567890", isTyping: false);

        await _httpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    #endregion

    #region Account Settings Tests

    [Fact]
    public async Task UpdateAccountSettingsAsync_DefaultSettings_CallsHttpClient()
    {
        _httpClientMock.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        await _client.UpdateAccountSettingsAsync();

        await _httpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateAccountSettingsAsync_DiscoverableFalse_CallsHttpClient()
    {
        _httpClientMock.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        await _client.UpdateAccountSettingsAsync(discoverableByNumber: false);

        await _httpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    #endregion

    #region Extension Methods Tests

    [Fact]
    public async Task SendMessageAsync_Extension_SendsCorrectRequest()
    {
        var expectedResponse = new SendMessage();
        _clientInterfaceMock.SendRequestAsync(Arg.Any<SendMessageRequest>(), Arg.Any<IQueryParameterRegistry?>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(expectedResponse));

        var result = await _clientInterfaceMock.SendMessageAsync("456", "hello");

        Assert.Same(expectedResponse, result);
        await _clientInterfaceMock.Received(1).SendRequestAsync(
            Arg.Is<SendMessageRequest>(r => r.Message == "hello" && r.Recipients!.Contains("456")),
            Arg.Any<IQueryParameterRegistry?>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAboutAsync_Extension_SendsGetAboutRequest()
    {
        var about = new About();
        _clientInterfaceMock
            .SendRequestAsync(Arg.Any<GetAboutRequest>(), Arg.Any<IQueryParameterRegistry?>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(about));

        var result = await _clientInterfaceMock.GetAboutAsync();

        Assert.Same(about, result);
    }

    #endregion

    #region Error Scenarios Tests

    [Fact]
    public async Task SendMessageAsync_WithHttpException_HandlesGracefully()
    {
        _httpClientMock.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new HttpRequestException());

        var expections = 0;
        using var t = _client.OnException.Subscribe(x =>
        {
            if (x is HttpRequestException)
            {
                expections++;
            }
        });
        await _client.SendMessageAsync(["+1234567890"], "Test message");

        await _httpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
        Assert.Equal(1, expections);
    }

    #endregion

    #region Configuration Tests

    [Fact]
    public void Client_Observables_NotNull()
    {
        Assert.NotNull(_client.OnApiRequest);
        Assert.NotNull(_client.OnApiResponse);
        Assert.NotNull(_client.OnException);
    }

    #endregion
}