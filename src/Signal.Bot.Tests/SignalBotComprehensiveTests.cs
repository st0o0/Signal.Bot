using System.Net;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Signal.Bot.Tests;

public class SignalBotComprehensiveTests : IAsyncDisposable
{
    private readonly WireMockServer _mockServer;
    private readonly SignalBotClient _client;
    private const string BotNumber = "+491701234567";
    private const string RecipientNumber = "+491709876543";
    private const string GroupId = "group.ckRzaEd4VmRzNnJaASAEsasa";

    public SignalBotComprehensiveTests()
    {
        _mockServer = WireMockServer.Start();
        _client = new SignalBotClient(x => x.WithBaseUrl(_mockServer.Url!).WithNumber(BotNumber));
    }

    public ValueTask DisposeAsync()
    {
        _mockServer?.Stop();
        _mockServer?.Dispose();
        return ValueTask.CompletedTask;
    }

    #region 1. Registration & Device Linking (V1 API)

    [Fact]
    public async Task RegisterNumber_WithCaptcha_ShouldSendVerificationCode()
    {
        // Arrange
        const string captchaToken = "signal-hcaptcha.xxxxx.registration.yyyyy";

        _mockServer
            .Given(Request.Create()
                .WithPath($"/v1/register/{BotNumber}")
                .UsingPost()
                .WithBody(new JsonMatcher(new
                {
                    captcha = captchaToken,
                    use_voice = false
                })))
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.Created)
                .WithHeader("Content-Type", "application/json")
                .WithBody("{}"));

        // Act
        await _client.RegisterNumberAsync(captcha: captchaToken, useVoice: false,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(_mockServer.LogEntries);
    }

    [Fact]
    public async Task VerifyRegistration_WithCode_ShouldComplete()
    {
        // Arrange
        const string verificationCode = "123456";
        const string pin = "1234";

        _mockServer
            .Given(Request.Create()
                .WithPath($"/v1/register/{BotNumber}/verify/{verificationCode}")
                .UsingPost()
                .WithBody(new JsonMatcher(new { pin })))
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.Created)
                .WithBodyAsJson(new { }));

        // Act
        await _client.VerifyNumberAsync(verificationCode, pin,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(_mockServer.LogEntries);
    }

    #endregion

    #region 2. Send Messages (V2 API)

    [Fact]
    public async Task SendMessage_SimpleText_ShouldReturnTimestamp()
    {
        // Arrange
        var dateTime = DateTimeOffset.UtcNow;
        var timestamp = dateTime.ToUnixTimeMilliseconds();


        _mockServer
            .Given(Request.Create()
                .WithPath("/v2/send")
                .UsingPost()
                .WithBody(new JsonMatcher(new
                {
                    message = "Hello World!",
                    number = BotNumber,
                    recipients = new[] { RecipientNumber }
                })))
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.Created)
                .WithHeader("Content-Type", "application/json")
                .WithBodyAsJson(new { timestamp }));

        // Act
        var result = await _client.SendMessageAsync("Hello World!", RecipientNumber,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dateTime, result.Timestamp);
    }

    [Theory]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.Unauthorized)]
    [InlineData(HttpStatusCode.NotFound)]
    [InlineData(HttpStatusCode.TooManyRequests)]
    public async Task SendMessage_ApiErrors_ShouldThrowWithStatusCode(HttpStatusCode statusCode)
    {
        // Arrange
        _mockServer
            .Given(Request.Create()
                .WithPath("/v2/send")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(statusCode)
                .WithBodyAsJson(new { error = "Error occurred" }));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<HttpRequestException>(async () =>
            await _client.SendMessageAsync("Test", RecipientNumber,
                cancellationToken: TestContext.Current.CancellationToken));

        Assert.Equal(statusCode, exception.StatusCode);
    }

    #endregion

    #region 3. Receive Messages (V1 API)

    //[Fact]
    public async Task ReceiveMessages_WithTimeout_ShouldReturnMessages()
    {
        // Arrange
        var messages = new[]
        {
            new
            {
                envelope = new
                {
                    source = RecipientNumber,
                    timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    dataMessage = new
                    {
                        timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                        message = "Hello Bot!",
                        expiresInSeconds = 0,
                        viewOnce = false
                    }
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
        //var result = await _client.Me(BotNumber);

        // Assert
        //Assert.NotNull(result);
        //Assert.Single(result);
        //Assert.Equal("Hello Bot!", result.First().Envelope.DataMessage.Message);
    }

    #endregion

    #region 4. Group Management (V1 API)

    [Fact]
    public async Task CreateGroup_WithBasicInfo_ShouldReturnGroupId()
    {
        // Arrange
        const string groupName = "Test Group";
        var members = new[] { RecipientNumber };

        _mockServer
            .Given(Request.Create()
                .WithPath($"/v1/groups/{BotNumber}")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.Created)
                .WithBodyAsJson(new
                {
                    id = GroupId,
                    name = groupName
                }));

        // Act
        var result = await _client.CreateGroupAsync(x => x.WithMembers(members).WithName(groupName),
            TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(GroupId, result.Id);
        Assert.Equal(groupName, result.Name);
    }

    [Fact]
    public async Task ListGroups_ShouldReturnAllGroups()
    {
        // Arrange
        var groups = new[]
        {
            new { id = "group-1", name = "Group 1" },
            new { id = "group-2", name = "Group 2" }
        };

        _mockServer
            .Given(Request.Create()
                .WithPath($"/v1/groups/{BotNumber}")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithBodyAsJson(groups));

        // Act
        var result = await _client.GetGroupsAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(2, result.Count());
    }

    #endregion

    #region 5. Reactions & Typing

    [Fact]
    public async Task SendReaction_ToMessage_ShouldSucceed()
    {
        // Arrange
        _mockServer
            .Given(Request.Create()
                .WithPath($"/v1/reactions/{BotNumber}")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.Created));

        // Act
        await _client.AddReactionAsync(recipient: RecipientNumber, reaction: "👍", targetAuthor: RecipientNumber,
            timestamp: DateTime.UtcNow, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(_mockServer.LogEntries);
    }

    [Fact]
    public async Task SendTypingIndicator_ShouldSucceed()
    {
        // Arrange
        _mockServer
            .Given(Request.Create()
                .WithPath($"/v1/typing-indicator/{BotNumber}")
                .UsingPut())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent));

        // Act
        await _client.SetTypingIndicatorAsync(RecipientNumber, isTyping: true,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(_mockServer.LogEntries);
    }

    #endregion

    #region 6. Profile Management

    [Fact]
    public async Task UpdateProfile_AllFields_ShouldSucceed()
    {
        // Arrange
        _mockServer
            .Given(Request.Create()
                .WithPath($"/v1/profiles/{BotNumber}")
                .UsingPut())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent));

        // Act
        await _client.UpdateProfileAsync(name: "Bot Name", about: "Bot description",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(_mockServer.LogEntries);
    }

    #endregion

    #region 7. Complex Scenarios

    //[Fact]
    public async Task CompleteConversationFlow_ShouldWorkEndToEnd()
    {
        // Setup: Receive message
        _mockServer
            .Given(Request.Create()
                .WithPath($"/v1/receive/{BotNumber}")
                .UsingGet())
            .InScenario("conversation")
            .WillSetStateTo("message-received")
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithBodyAsJson(new[]
                {
                    new
                    {
                        envelope = new
                        {
                            source = RecipientNumber,
                            timestamp = 111111111L,
                            dataMessage = new
                            {
                                timestamp = 111111111L,
                                message = "Hello Bot!"
                            }
                        }
                    }
                }));

        // Setup: Send typing
        _mockServer
            .Given(Request.Create()
                .WithPath($"/v1/typing-indicator/{BotNumber}")
                .UsingPut())
            .InScenario("conversation")
            .WhenStateIs("message-received")
            .WillSetStateTo("typing")
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent));

        // Setup: Send reply
        _mockServer
            .Given(Request.Create()
                .WithPath("/v2/send")
                .UsingPost())
            .InScenario("conversation")
            .WhenStateIs("typing")
            .WillSetStateTo("complete")
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.Created)
                .WithBodyAsJson(new { timestamp = 222222222L }));

        // Execute
        //await _client.ReceiveMessagesAsync(BotNumber);
        //await _client.SendTypingIndicatorAsync(BotNumber, RecipientNumber);
        //await _client.SendMessageAsync(BotNumber, "Hi!", new[] { RecipientNumber });

        // Assert
        Assert.Equal(3, _mockServer.LogEntries.Count());
        //Assert.Equal("complete", _mockServer.Scenarios["conversation"].CurrentState);
    }

    #endregion
}