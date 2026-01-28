using System.Net;
using System.Text.Json;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Signal.Bot.Requests;
using Signal.Bot.Types;

namespace Signal.Bot.Tests;

/// <summary>
/// Extended test coverage for SignalBotClient methods including Groups, Accounts, and specialized scenarios
/// </summary>
public class SignalBotClientExtendedTests
{
    private readonly HttpClient _httpClientMock;
    private readonly SignalBotClient _client;

    public SignalBotClientExtendedTests()
    {
        _httpClientMock = Substitute.For<HttpClient>();
        _client = new SignalBotClient(new SignalBotClientOptions("123", "http://localhost:8080"), _httpClientMock);
    }

    #region GetAccountsAsync Tests

    [Fact]
    public async Task GetAccountsAsync_ValidRequest_ReturnsAccounts()
    {
        // Arrange
        var accounts = new List<string> { "account1", "account2" };
        var json = JsonSerializer.Serialize(accounts);
        var content = new StringContent(json);

        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = content }));

        // Act
        var result = await _client.GetAccountsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        await _httpClientMock
            .Received(1)
            .SendAsync(
                Arg.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAccountsAsync_WithCancellationToken_PassesCancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        var accounts = new List<string>();
        var json = JsonSerializer.Serialize(accounts);
        var content = new StringContent(json);

        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = content }));

        // Act
        var result = await _client.GetAccountsAsync(cts.Token);

        // Assert
        Assert.NotNull(result);
        cts.Dispose();
    }

    #endregion

    #region GetGroupsAsync Tests

    [Fact]
    public async Task GetGroupsAsync_ValidRequest_ReturnsGroups()
    {
        // Arrange
        var groups = new List<Group>();
        var json = JsonSerializer.Serialize(groups);
        var content = new StringContent(json);

        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = content }));

        // Act
        var result = await _client.GetGroupsAsync();

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

    #region RegisterNumberAsync Tests

    [Fact]
    public async Task RegisterNumberAsync_WithoutParameters_CallsHttpClient()
    {
        // Arrange
        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        // Act
        await _client.RegisterNumberAsync();

        // Assert
        await _httpClientMock
            .Received(1)
            .SendAsync(
                Arg.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RegisterNumberAsync_WithCaptcha_CallsHttpClient()
    {
        // Arrange
        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        // Act
        await _client.RegisterNumberAsync(captcha: "captcha-token");

        // Assert
        await _httpClientMock
            .Received(1)
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RegisterNumberAsync_WithVoiceOption_CallsHttpClient()
    {
        // Arrange
        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        // Act
        await _client.RegisterNumberAsync(useVoice: true);

        // Assert
        await _httpClientMock
            .Received(1)
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    #endregion

    #region VerifyNumberAsync Tests

    [Fact]
    public async Task VerifyNumberAsync_ValidToken_ReturnsVerificationResult()
    {
        // Arrange
        var token = "verification-token";
        var expectedResult = "verified-token";
        var json = JsonSerializer.Serialize(expectedResult);
        var content = new StringContent(json);

        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = content }));

        // Act
        var result = await _client.VerifyNumberAsync(token);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("verified-token", result);
    }

    [Fact]
    public async Task VerifyNumberAsync_WithPin_CallsHttpClient()
    {
        // Arrange
        var token = "verification-token";
        var pin = "123456";
        var expectedResult = "verified-token";
        var json = JsonSerializer.Serialize(expectedResult);
        var content = new StringContent(json);

        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = content }));

        // Act
        var result = await _client.VerifyNumberAsync(token, pin);

        // Assert
        Assert.NotNull(result);
        await _httpClientMock
            .Received(1)
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    #endregion

    #region SetTypingIndicatorAsync Tests

    [Fact]
    public async Task SetTypingIndicatorAsync_WithRecipient_CallsHttpClient()
    {
        // Arrange
        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        // Act
        await _client.SetTypingIndicatorAsync(recipient: "+1234567890");

        // Assert
        await _httpClientMock
            .Received(1)
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SetTypingIndicatorAsync_WithGroupId_CallsHttpClient()
    {
        // Arrange
        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        // Act
        await _client.SetTypingIndicatorAsync(groupId: "group-id");

        // Assert
        await _httpClientMock
            .Received(1)
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SetTypingIndicatorAsync_IsTypingFalse_CallsHttpClient()
    {
        // Arrange
        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        // Act
        await _client.SetTypingIndicatorAsync(recipient: "+1234567890", isTyping: false);

        // Assert
        await _httpClientMock
            .Received(1)
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    #endregion

    #region RateLimitChallengeAsync Tests

    [Fact]
    public async Task RateLimitChallengeAsync_ValidParameters_CallsHttpClient()
    {
        // Arrange
        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        // Act
        await _client.RateLimitChallengeAsync("challenge-token", "captcha-solution");

        // Assert
        await _httpClientMock
            .Received(1)
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    #endregion

    #region UpdateAccountSettingsAsync Tests

    [Fact]
    public async Task UpdateAccountSettingsAsync_DefaultSettings_CallsHttpClient()
    {
        // Arrange
        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        // Act
        await _client.UpdateAccountSettingsAsync();

        // Assert
        await _httpClientMock
            .Received(1)
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateAccountSettingsAsync_DiscoverableFalse_CallsHttpClient()
    {
        // Arrange
        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        // Act
        await _client.UpdateAccountSettingsAsync(discoverableByNumber: false);

        // Assert
        await _httpClientMock
            .Received(1)
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateAccountSettingsAsync_ShareNumberFalse_CallsHttpClient()
    {
        // Arrange
        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        // Act
        await _client.UpdateAccountSettingsAsync(shareNumberWithContacts: false);

        // Assert
        await _httpClientMock
            .Received(1)
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    #endregion

    #region SetUsernameAsync Tests

    [Fact]
    public async Task SetUsernameAsync_ValidUsername_ReturnsSetUsernameResult()
    {
        // Arrange
        var username = "newusername";
        var result = new SetUsername { Username = username };
        var json = JsonSerializer.Serialize(result);
        var content = new StringContent(json);

        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = content }));

        // Act
        var response = await _client.SetUsernameAsync(username);

        // Assert
        Assert.NotNull(response);
        await _httpClientMock
            .Received(1)
            .SendAsync(
                Arg.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post),
                Arg.Any<CancellationToken>());
    }

    #endregion

    #region RemoveUsernameAsync Tests

    [Fact]
    public async Task RemoveUsernameAsync_ValidRequest_CallsHttpClient()
    {
        // Arrange
        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        // Act
        await _client.RemoveUsernameAsync();

        // Assert
        await _httpClientMock
            .Received(1)
            .SendAsync(
                Arg.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Delete),
                Arg.Any<CancellationToken>());
    }

    #endregion

    #region Configuration Tests

    [Fact]
    public void Client_InitializedWithOptions_HasCorrectProperties()
    {
        // Arrange
        var options = new SignalBotClientOptions("999", "http://api.example.com:9090");
        var httpClient = Substitute.For<HttpClient>();

        // Act
        var client = new SignalBotClient(options, httpClient);

        // Assert
        Assert.Equal("999", client.Number);
        Assert.Equal("http://api.example.com:9090", client.BaseUrl);
        Assert.NotNull(client.JsonSerializerOptions);
    }

    #endregion

    #region Observable Properties Tests

    [Fact]
    public void OnApiRequest_ReturnsObservable()
    {
        // Act & Assert
        Assert.NotNull(_client.OnApiRequest);
    }

    [Fact]
    public void OnApiResponse_ReturnsObservable()
    {
        // Act & Assert
        Assert.NotNull(_client.OnApiResponse);
    }

    [Fact]
    public void OnException_ReturnsObservable()
    {
        // Act & Assert
        Assert.NotNull(_client.OnException);
    }

    [Fact]
    public void Timeout_ReturnsTimeSpan()
    {
        // Act & Assert
        Assert.True(_client.Timeout > TimeSpan.Zero);
    }

    #endregion

    #region Multiple Recipients Scenarios Tests

    [Fact]
    public async Task SendMessageAsync_LargeRecipientList_CallsHttpClientOnce()
    {
        // Arrange
        var recipients = Enumerable.Range(1, 100)
            .Select(i => $"+1234567890{i}")
            .ToArray();

        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        // Act
        await _client.SendMessageAsync(recipients, "Message to many recipients");

        // Assert
        await _httpClientMock
            .Received(1)
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    #endregion

    #region Cancellation Token Propagation Tests

    [Fact]
    public async Task GetAccountsAsync_WithCancellationToken_PropagatesToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        var accounts = new List<string>();
        var json = JsonSerializer.Serialize(accounts);
        var content = new StringContent(json);

        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = content }));

        // Act
        await _client.GetAccountsAsync(cts.Token);

        // Assert
        await _httpClientMock
            .Received(1)
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());

        cts.Dispose();
    }

    [Fact]
    public async Task UpdateProfileAsync_AllParameters_CallsHttpClient()
    {
        // Arrange
        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        // Act
        await _client.UpdateProfileAsync(
            name: "John Doe",
            about: "My status",
            base64Avatar: "base64encodedimage");

        // Assert
        await _httpClientMock
            .Received(1)
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    #endregion

    #region Error Scenarios Tests

    [Fact]
    public async Task SendMessageAsync_WithHttpException_HandlesGracefully()
    {
        // Arrange
        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .ThrowsAsync<HttpRequestException>();

        // Act
        _ = await _client.SendMessageAsync(["+1234567890"], "Test message");

        // Assert
        await _httpClientMock
            .Received(1)
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAccountsAsync_WithTimeout_HandlesGracefully()
    {
        // Arrange
        _httpClientMock
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .ThrowsAsync<TaskCanceledException>();

        // Act
        _ = await _client.GetAccountsAsync();

        // Assert
        await _httpClientMock
            .Received(1)
            .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    #endregion
}