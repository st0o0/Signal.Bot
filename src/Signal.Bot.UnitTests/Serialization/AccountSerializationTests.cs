using System.Text.Json;
using Signal.Bot.Requests;

namespace Signal.Bot.UnitTests.Serialization;

public class AccountSerializationTests
{
    [Fact(Timeout = 5000)]
    public void TestRegisterNumberRequestSerializationAndDeserialization()
    {
        // Arrange
        var registerNumberRequest = new RegisterNumberRequest("")
        {
            Captcha = "+1234567890",
            UseVoice = false
        };

        // Act
        var json = JsonSerializer.Serialize(registerNumberRequest);
        var deserializedRegisterNumberRequest = JsonSerializer.Deserialize<RegisterNumberRequest>(json);

        // Assert
        Assert.NotNull(deserializedRegisterNumberRequest);
        Assert.Equal(registerNumberRequest.UseVoice, deserializedRegisterNumberRequest.UseVoice);
    }

    [Fact(Timeout = 5000)]
    public void TestVerifyNumberRequestSerializationAndDeserialization()
    {
        // Arrange
        var verifyNumberRequest = new VerifyNumberRequest("", "")
        {
            Pin = "jdafhlksjd"
        };

        // Act
        var json = JsonSerializer.Serialize(verifyNumberRequest);
        var deserializedVerifyNumberRequest = JsonSerializer.Deserialize<VerifyNumberRequest>(json);

        // Assert
        Assert.NotNull(deserializedVerifyNumberRequest);
        Assert.Equal(verifyNumberRequest.Pin, deserializedVerifyNumberRequest.Pin);
    }

    [Fact(Timeout = 5000)]
    public void TestRateLimitChallengeRequestSerializationAndDeserialization()
    {
        // Arrange
        var rateLimitChallengeRequest = new RateLimitChallengeRequest("")
        {
            Captcha = "challenge123",
            ChallengeToken = "challengeResponse456"
        };

        // Act
        var json = JsonSerializer.Serialize(rateLimitChallengeRequest);
        var deserializedRateLimitChallengeRequest = JsonSerializer.Deserialize<RateLimitChallengeRequest>(json);

        // Assert
        Assert.NotNull(deserializedRateLimitChallengeRequest);
        Assert.Equal(rateLimitChallengeRequest.Captcha, deserializedRateLimitChallengeRequest.Captcha);
        Assert.Equal(rateLimitChallengeRequest.ChallengeToken, deserializedRateLimitChallengeRequest.ChallengeToken);
    }

    [Fact(Timeout = 5000)]
    public void TestUpdateAccountSettingsRequestSerializationAndDeserialization()
    {
        // Arrange
        var updateAccountSettingsRequest = new UpdateAccountSettingsRequest("")
        {
            DiscoverableByNumber = false,
            ShareNumber = false,
        };

        // Act
        var json = JsonSerializer.Serialize(updateAccountSettingsRequest);
        var deserializedUpdateAccountSettingsRequest = JsonSerializer.Deserialize<UpdateAccountSettingsRequest>(json);

        // Assert
        Assert.NotNull(deserializedUpdateAccountSettingsRequest);
        Assert.Equal(updateAccountSettingsRequest.DiscoverableByNumber,
            deserializedUpdateAccountSettingsRequest.DiscoverableByNumber);
        Assert.Equal(updateAccountSettingsRequest.ShareNumber, deserializedUpdateAccountSettingsRequest.ShareNumber);
    }
}

