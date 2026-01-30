using System.Text.Json;
using Signal.Bot.Requests;
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
            Created = 2387149324,
            LastSeen = 239417043928
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
    public void TestReactionMessageSerializationAndDeserialization()
    {
        // Arrange
        var reactionMessage = new ReactionMessage
        {
            IsRemove = false,
            Emoji = "👍"
        };

        // Act
        var json = JsonSerializer.Serialize(reactionMessage);
        var deserializedReactionMessage = JsonSerializer.Deserialize<ReactionMessage>(json);

        // Assert
        Assert.NotNull(deserializedReactionMessage);
        Assert.Equal(reactionMessage.IsRemove, deserializedReactionMessage.IsRemove);
        Assert.Equal(reactionMessage.Emoji, deserializedReactionMessage.Emoji);
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
                SourceUuid = "msg123",
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
        var remoteDeleteMessage = new RemoteDeleteMessage
        {
            TargetSentTimestamp = 12314
        };

        // Act
        var json = JsonSerializer.Serialize(remoteDeleteMessage);
        var deserializedRemoteDeleteMessage = JsonSerializer.Deserialize<RemoteDeleteMessage>(json);

        // Assert
        Assert.NotNull(deserializedRemoteDeleteMessage);
        Assert.Equal(remoteDeleteMessage.TargetSentTimestamp, deserializedRemoteDeleteMessage.TargetSentTimestamp);
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
}