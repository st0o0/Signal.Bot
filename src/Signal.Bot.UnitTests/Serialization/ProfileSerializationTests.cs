using System.Text.Json;
using Signal.Bot.Requests;
using Signal.Bot.Types;

namespace Signal.Bot.UnitTests.Serialization;

public class ProfileSerializationTests
{
    [Fact(Timeout = 5000)]
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

    [Fact(Timeout = 5000)]
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

    [Fact(Timeout = 5000)]
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

    [Fact(Timeout = 5000)]
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
}

