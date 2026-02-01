using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Signal.Bot.Requests;
using Signal.Bot.Serialization;
using Signal.Bot.Types;

namespace Signal.Bot.Tests;

public class TypeSerializationTests
{
    [Fact]
    public void TestAddDeviceRequestSerializationAndDeserialization()
    {
        // Arrange
        var addDeviceRequest = new AddDeviceRequest("")
        {
            Uri = "TEST123"
        };

        // Act
        var json = JsonSerializer.Serialize(addDeviceRequest);
        var deserializedAddDeviceRequest = JsonSerializer.Deserialize<AddDeviceRequest>(json);

        // Assert
        Assert.NotNull(deserializedAddDeviceRequest);
        Assert.Equal(addDeviceRequest.Uri, deserializedAddDeviceRequest.Uri);
    }

    [Fact]
    public void TestContactSerializationAndDeserialization()
    {
        // Arrange
        var contact = new Contact
        {
            Number = "+1234567890",
            Name = "John Doe"
        };

        // Act
        var json = JsonSerializer.Serialize(contact);
        var deserializedContact = JsonSerializer.Deserialize<Contact>(json);

        // Assert
        Assert.NotNull(deserializedContact);
        Assert.Equal(contact.Number, deserializedContact.Number);
        Assert.Equal(contact.Name, deserializedContact.Name);
    }

    [Fact]
    public void TestDeviceSerializationAndDeserialization()
    {
        // Arrange
        var device = new Device
        {
            Name = "My Device",
            Created = DateTimeOffset.FromUnixTimeMilliseconds(2387149324).DateTime,
            LastSeen = DateTimeOffset.FromUnixTimeMilliseconds(239417043928).DateTime
        };

        // Act
        var json = JsonSerializer.Serialize(device);
        var deserializedDevice = JsonSerializer.Deserialize<Device>(json);

        // Assert
        Assert.NotNull(deserializedDevice);
        Assert.Equal(device.Name, deserializedDevice.Name);
        Assert.Equal(device.Created, deserializedDevice.Created);
        Assert.Equal(device.LastSeen, deserializedDevice.LastSeen);
    }

    [Fact]
    public void TestErrorSerializationAndDeserialization()
    {
        // Arrange
        var error = new Types.Error
        {
            Message = "Not Found"
        };

        // Act
        var json = JsonSerializer.Serialize(error);
        var deserializedError = JsonSerializer.Deserialize<Types.Error>(json);

        // Assert
        Assert.NotNull(deserializedError);
        Assert.Equal(error.Message, deserializedError.Message);
    }

    [Fact]
    public void TestNicknameSerializationAndDeserialization()
    {
        // Arrange
        var nickname = new Nickname
        {
            Name = "Nick",
            FamilyName = "Nick",
            GivenName = "Nick"
        };

        // Act
        var json = JsonSerializer.Serialize(nickname);
        var deserializedNickname = JsonSerializer.Deserialize<Nickname>(json);

        // Assert
        Assert.NotNull(deserializedNickname);
        Assert.Equal(nickname.Name, deserializedNickname.Name);
        Assert.Equal(nickname.FamilyName, deserializedNickname.FamilyName);
        Assert.Equal(nickname.GivenName, deserializedNickname.GivenName);
    }

    [Fact]
    public void TestProfileSerializationAndDeserialization()
    {
        // Arrange
        var profile = new Profile
        {
            About = "uuid123",
            GivenName = "John Doe"
        };

        // Act
        var json = JsonSerializer.Serialize(profile);
        var deserializedProfile = JsonSerializer.Deserialize<Profile>(json);

        // Assert
        Assert.NotNull(deserializedProfile);
        Assert.Equal(profile.About, deserializedProfile.About);
        Assert.Equal(profile.GivenName, deserializedProfile.GivenName);
    }

    [Fact]
    public void TestProfileCapabilitiesSerializationAndDeserialization()
    {
        // Arrange
        var profileCapabilities = new ProfileCapabilities
        {
            ChangeNumber = true,
            GiftBadges = false
        };

        // Act
        var json = JsonSerializer.Serialize(profileCapabilities);
        var deserializedProfileCapabilities = JsonSerializer.Deserialize<ProfileCapabilities>(json);

        // Assert
        Assert.NotNull(deserializedProfileCapabilities);
        Assert.Equal(profileCapabilities.ChangeNumber, deserializedProfileCapabilities.ChangeNumber);
        Assert.Equal(profileCapabilities.GiftBadges, deserializedProfileCapabilities.GiftBadges);
    }

    [Fact]
    public void TestReceivedMessageSerializationAndDeserialization()
    {
        // Arrange
        var receivedMessage = new ReceivedMessage
        {
            Account = "msg123",
            Envelope = new Envelope
            {
                SourceId = Guid.Empty,
                SourceNumber = "msg123",
                Source = "msg123",
                DataMessage = new DataMessage
                {
                    Body = "Hello, World!"
                }
            }
        };

        // Act
        var json = JsonSerializer.Serialize(receivedMessage);
        var deserializedReceivedMessage = JsonSerializer.Deserialize<ReceivedMessage>(json);

        // Assert
        Assert.NotNull(deserializedReceivedMessage);
        Assert.NotNull(deserializedReceivedMessage.Envelope);
        Assert.NotNull(deserializedReceivedMessage.Envelope.DataMessage);
        Assert.Equal(receivedMessage.Account, deserializedReceivedMessage.Account);
        Assert.Equal(receivedMessage.Envelope.DataMessage.Body, deserializedReceivedMessage.Envelope.DataMessage.Body);
    }

    [Fact]
    public void TestRemoteDeleteMessageSerializationAndDeserialization()
    {
        // Arrange
        var remoteDeleteMessage = new Acknowledged
        {
            Timestamp = DateTime.Now
        };

        // Act
        var json = JsonSerializer.Serialize(remoteDeleteMessage);
        var deserializedRemoteDeleteMessage = JsonSerializer.Deserialize<Acknowledged>(json);

        // Assert
        Assert.NotNull(deserializedRemoteDeleteMessage);
        Assert.Equal(remoteDeleteMessage.Timestamp, deserializedRemoteDeleteMessage.Timestamp);
    }

    [Fact]
    public void TestAddGroupMemberRequestSerializationAndDeserialization()
    {
        // Arrange
        var addGroupMemberRequest = new AddGroupMemberRequest("", "")
        {
            Members = ["memberUuid456"]
        };

        // Act
        var json = JsonSerializer.Serialize(addGroupMemberRequest);
        var deserializedAddGroupMemberRequest = JsonSerializer.Deserialize<AddGroupMemberRequest>(json);

        // Assert
        Assert.NotNull(deserializedAddGroupMemberRequest);
        Assert.NotNull(deserializedAddGroupMemberRequest.Members);
        Assert.NotEmpty(deserializedAddGroupMemberRequest.Members);
        Assert.Contains(addGroupMemberRequest.Members.ToArray(), deserializedAddGroupMemberRequest.Members.ToArray());
    }

    [Fact]
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

    [Fact]
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

    [Fact]
    public void TestSendMessageRequestSerializationAndDeserialization()
    {
        // Arrange
        var sendMessageRequest = new SendMessageRequest
        {
            Message = "recipientUuid123",
            QuoteAuthor = "Hello, World!"
        };

        // Act
        var json = JsonSerializer.Serialize(sendMessageRequest);
        var deserializedSendMessageRequest = JsonSerializer.Deserialize<SendMessageRequest>(json);

        // Assert
        Assert.NotNull(deserializedSendMessageRequest);
        Assert.Equal(sendMessageRequest.Message, deserializedSendMessageRequest.Message);
        Assert.Equal(sendMessageRequest.QuoteAuthor, deserializedSendMessageRequest.QuoteAuthor);
    }

    [Fact]
    public void TestSetConfigurationRequestSerializationAndDeserialization()
    {
        // Arrange
        var setConfigurationRequest = new SetConfigurationRequest
        {
            Logging = "configKey123",
        };

        // Act
        var json = JsonSerializer.Serialize(setConfigurationRequest);
        var deserializedSetConfigurationRequest = JsonSerializer.Deserialize<SetConfigurationRequest>(json);

        // Assert
        Assert.NotNull(deserializedSetConfigurationRequest);
        Assert.Equal(setConfigurationRequest.Logging, deserializedSetConfigurationRequest.Logging);
    }

    [Fact]
    public void TestSetTypingIndicatorRequestSerializationAndDeserialization()
    {
        // Arrange
        var setTypingIndicatorRequest = new SetTypingIndicatorRequest("")
        {
            GroupId = "groupId123",
            Recipient = "recipientUuid456",
        };

        // Act
        var json = JsonSerializer.Serialize(setTypingIndicatorRequest);
        var deserializedSetTypingIndicatorRequest = JsonSerializer.Deserialize<SetTypingIndicatorRequest>(json);

        // Assert
        Assert.NotNull(deserializedSetTypingIndicatorRequest);
        Assert.Equal(setTypingIndicatorRequest.GroupId, deserializedSetTypingIndicatorRequest.GroupId);
        Assert.Equal(setTypingIndicatorRequest.Recipient, deserializedSetTypingIndicatorRequest.Recipient);
    }

    [Fact]
    public void TestUpdateAccountSettingsRequestSerializationAndDeserialization()
    {
        // Arrange
        var updateAccountSettingsRequest = new UpdateAccountSettingsRequest("")
        {
            DiscoverableByNumber = false,
            ShareNumberWithContacts = false,
        };

        // Act
        var json = JsonSerializer.Serialize(updateAccountSettingsRequest);
        var deserializedUpdateAccountSettingsRequest = JsonSerializer.Deserialize<UpdateAccountSettingsRequest>(json);

        // Assert
        Assert.NotNull(deserializedUpdateAccountSettingsRequest);
        Assert.Equal(updateAccountSettingsRequest.DiscoverableByNumber,
            deserializedUpdateAccountSettingsRequest.DiscoverableByNumber);
        Assert.Equal(updateAccountSettingsRequest.ShareNumberWithContacts,
            deserializedUpdateAccountSettingsRequest.ShareNumberWithContacts);
    }

    [Fact]
    public void TestUpdateContactRequestSerializationAndDeserialization()
    {
        // Arrange
        var updateContactRequest = new UpdateContactRequest("")
        {
            Name = "updateContactRequestName123",
            Recipient = "updateContactRequestNickname123",
        };

        // Act
        var json = JsonSerializer.Serialize(updateContactRequest);
        var deserializedUpdateContactRequest = JsonSerializer.Deserialize<UpdateContactRequest>(json);

        // Assert
        Assert.NotNull(deserializedUpdateContactRequest);
        Assert.Equal(updateContactRequest.Name, deserializedUpdateContactRequest.Name);
        Assert.Equal(updateContactRequest.Recipient, deserializedUpdateContactRequest.Recipient);
    }

    [Fact]
    public void TestUpdateProfileRequestSerializationAndDeserialization()
    {
        // Arrange
        var updateProfileRequest = new UpdateProfileRequest("")
        {
            About = "uuid123",
            Name = "John Doe"
        };

        // Act
        var json = JsonSerializer.Serialize(updateProfileRequest);
        var deserializedUpdateProfileRequest = JsonSerializer.Deserialize<UpdateProfileRequest>(json);

        // Assert
        Assert.NotNull(deserializedUpdateProfileRequest);
        Assert.Equal(updateProfileRequest.About, deserializedUpdateProfileRequest.About);
        Assert.Equal(updateProfileRequest.Name, deserializedUpdateProfileRequest.Name);
    }

    [Fact]
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

    [Fact]
    public void TestRemoveGroupMemberRequestSerializationAndDeserialization()
    {
        // Arrange
        var removeGroupMemberRequest = new RemoveGroupMemberRequest("", "")
        {
            Members = ["members"]
        };

        // Act
        var json = JsonSerializer.Serialize(removeGroupMemberRequest);
        var deserializedRemoveGroupMemberRequest = JsonSerializer.Deserialize<RemoveGroupMemberRequest>(json);

        // Assert
        Assert.NotNull(deserializedRemoveGroupMemberRequest);
        Assert.NotNull(deserializedRemoveGroupMemberRequest.Members);
        Assert.Contains(removeGroupMemberRequest.Members.ToArray(),
            deserializedRemoveGroupMemberRequest.Members.ToArray());
    }

    [Fact]
    public void Deserialize_WithAllProperties_ShouldSucceed()
    {
        // Arrange
        const string json = """
                            {
                                "added": 1769013962970,
                                "fingerprint": "05 b8 fc 5b 4c 3e 67 5d 9d 40 bc 82 bc aa 0a 1b ea ca be 39 dd b6 63 b7 71 96 2f 34 4d 75 3e 18 5c",
                                "number": "+1234567890",
                                "safety_number": "123456",
                                "status": "TRUSTED_VERIFIED",
                                "uuid": "5cc79553-6f51-4ee0-bf9f-5b99b682da5f"
                            }
                            """;

        // Act
        var identity = JsonSerializer.Deserialize<Identity>(json, JsonBotAPI.Options);

        // Assert
        Assert.NotNull(identity);
        Assert.Equal(
            "05 b8 fc 5b 4c 3e 67 5d 9d 40 bc 82 bc aa 0a 1b ea ca be 39 dd b6 63 b7 71 96 2f 34 4d 75 3e 18 5c",
            identity.Fingerprint);
        Assert.Equal("+1234567890", identity.Number);
        Assert.Equal("123456", identity.SafetyNumber);
        Assert.Equal(IdentityStatus.TrustedVerified, identity.Status);
        Assert.Equal(Guid.Parse("5cc79553-6f51-4ee0-bf9f-5b99b682da5f"), identity.Id);
    }

    [Fact]
    public void Deserialize_WithNullableProperties_ShouldSucceed()
    {
        // Arrange
        const string json = """
                            {
                                "number": "+1234567890"
                            }
                            """;

        // Act
        var identity = JsonSerializer.Deserialize<Identity>(json, JsonBotAPI.Options);

        // Assert
        Assert.NotNull(identity);
        Assert.Null(identity.Fingerprint);
        Assert.Equal("+1234567890", identity.Number);
        Assert.Null(identity.SafetyNumber);
    }

    [Theory]
    [InlineData("UNDEFINED", IdentityStatus.Undefined)]
    [InlineData("UNTRUSTED", IdentityStatus.Untrusted)]
    [InlineData("TRUSTED_UNVERIFIED", IdentityStatus.TrustedUnverified)]
    [InlineData("TRUSTED_VERIFIED", IdentityStatus.TrustedVerified)]
    public void Deserialize_WithDifferentStatuses_ShouldParseCorrectly(string statusString,
        IdentityStatus expectedStatus)
    {
        // Arrange
        var json = $$"""{"status": "{{statusString}}"}""";

        // Act
        var identity = JsonSerializer.Deserialize<Identity>(json, JsonBotAPI.Options);

        // Assert
        Assert.NotNull(identity);
        Assert.Equal(expectedStatus, identity.Status);
    }

    [Fact]
    public void Serialize_WithAllProperties_ShouldSucceed()
    {
        // Arrange
        var identity = new Identity
        {
            Added = DateTime.Parse("2026-01-31T12:12:42.970Z", CultureInfo.CurrentCulture),
            Fingerprint =
                "05 b8 fc 5b 4c 3e 67 5d 9d 40 bc 82 bc aa 0a 1b ea ca be 39 dd b6 63 b7 71 96 2f 34 4d 75 3e 18 5c",
            Number = "+1234567890",
            SafetyNumber = "123456",
            Status = IdentityStatus.TrustedVerified,
            Id = Guid.Parse("d57809cb-648a-4a8e-b45f-e3d488f6696b")
        };

        // Act
        var json = JsonSerializer.Serialize(identity, JsonBotAPI.Options);

        // Assert
        Assert.Contains("\"added\":", json);
        Assert.Contains("\"fingerprint\":", json);
        Assert.Contains("\"number\":", json);
        Assert.Contains("\"safety_number\"", json);
        Assert.Contains("\"status\":", json);
        Assert.Contains("\"uuid\":", json);
    }

    [Fact]
    public void Serialize_WithDefaultProperties_ShouldOmitNulls()
    {
        // Arrange
        var identity = new Identity
        {
            Number = "+1234567890"
        };

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
        };

        // Act
        var json = JsonSerializer.Serialize(identity, options);

        // Assert
        Assert.Contains("""{"number":"\u002B1234567890"}""", json);
        Assert.DoesNotContain("\"added\":", json);
        Assert.DoesNotContain("\"fingerprint\":", json);
        Assert.DoesNotContain("\"safety_number\":", json);
        Assert.DoesNotContain("\"status\":", json);
        Assert.DoesNotContain("\"uuid\":", json);
    }

    [Fact]
    public void Deserialize_WithInvalidGuid_ShouldThrowException()
    {
        // Arrange
        const string json = """
                            {
                                "uuid": "invalid-guid"
                            }
                            """;

        // Act & Assert
        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<Identity>(json, JsonBotAPI.Options));
    }

    [Fact]
    public void Deserialize_WithInvalidStatus_ShouldThrowException()
    {
        // Arrange
        const string json = """
                            {
                                "status": "INVALID_STATUS"
                            }
                            """;

        // Act & Assert
        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<Identity>(json, JsonBotAPI.Options));
    }

    [Fact]
    public void RoundTrip_ShouldPreserveData()
    {
        // Arrange
        var original = new Identity
        {
            Added = DateTime.UtcNow,
            Fingerprint = "test fingerprint",
            Number = "+1234567890",
            SafetyNumber = "123456",
            Status = IdentityStatus.TrustedUnverified,
            Id = Guid.NewGuid()
        };

        // Act
        var json = JsonSerializer.Serialize(original, JsonBotAPI.Options);
        var deserialized = JsonSerializer.Deserialize<Identity>(json, JsonBotAPI.Options);

        // Assert
        Assert.NotNull(deserialized);
        var tolerance = TimeSpan.FromMilliseconds(1).Ticks;
        Assert.InRange(
            deserialized.Added.Ticks,
            original.Added.Ticks - tolerance,
            original.Added.Ticks + tolerance
        );
        Assert.Equal(original.Fingerprint, deserialized.Fingerprint);
        Assert.Equal(original.Number, deserialized.Number);
        Assert.Equal(original.SafetyNumber, deserialized.SafetyNumber);
        Assert.Equal(original.Status, deserialized.Status);
        Assert.Equal(original.Id, deserialized.Id);
    }

    [Fact]
    public void Identity_DefaultValues_ShouldBeNull()
    {
        // Act
        var identity = new Identity();

        // Assert
        Assert.Equal(DateTime.MinValue, identity.Added);
        Assert.Null(identity.Fingerprint);
        Assert.Null(identity.Number);
        Assert.Null(identity.SafetyNumber);
        Assert.Equal(IdentityStatus.Undefined, identity.Status);
        Assert.Equal(Guid.Empty, identity.Id);
    }
}