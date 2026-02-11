using System.Net;
using Signal.Bot.IntegrationTests.Utils;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Signal.Bot.IntegrationTests.Extensions;

public class RegistrationTests : IntegrationTestBase
{
    [Fact(Timeout = 15000)]
    public async Task RegisterNumber_WithCaptcha_ShouldSendVerificationCode()
    {
        // Arrange
        const string captchaToken = "signal-hcaptcha.xxxxx.registration.yyyyy";

        MockServer
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
        await Client.RegisterNumberAsync(captcha: captchaToken, useVoice: false,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(MockServer.LogEntries);
    }

    [Fact(Timeout = 15000)]
    public async Task VerifyRegistration_WithCode_ShouldComplete()
    {
        // Arrange
        const string verificationCode = "123456";
        const string pin = "1234";

        MockServer
            .Given(Request.Create()
                .WithPath($"/v1/register/{BotNumber}/verify/{verificationCode}")
                .UsingPost()
                .WithBody(new JsonMatcher(new { pin })))
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.Created)
                .WithBodyAsJson(new { }));

        // Act
        await Client.VerifyNumberAsync(verificationCode, pin,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(MockServer.LogEntries);
    }

    [Theory(Timeout = 15000)]
    [InlineData("123456")]
    [InlineData("not-a-number")]
    [InlineData("++491701234567")]
    public async Task RegisterNumber_InvalidFormat_ShouldNotThrow(string invalidNumber)
    {
        // Arrange
        MockServer
            .Given(Request.Create()
                .WithPath($"/v1/register/{invalidNumber}")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.BadRequest)
                .WithBodyAsJson(new { error = "Invalid phone number format" }));

        // Act
        await Client.RegisterNumberAsync(invalidNumber, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(MockServer.LogEntries);
    }

    [Fact(Timeout = 15000)]
    public async Task UnregisterDevice_ShouldSucceed()
    {
        // Arrange
        MockServer
            .Given(Request.Create()
                .WithPath($"/v1/unregister/{BotNumber}")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK));

        // Act
        await Client.UnregisterDeviceAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(MockServer.LogEntries);
    }
}

