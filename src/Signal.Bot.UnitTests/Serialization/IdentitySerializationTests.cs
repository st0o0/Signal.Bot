using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Signal.Bot.Serialization;
using Signal.Bot.Types;

namespace Signal.Bot.UnitTests.Serialization;

public class IdentitySerializationTests
{
    [Fact(Timeout = 5000)]
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

    [Fact(Timeout = 5000)]
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

    [Theory(Timeout = 5000)]
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

    [Fact(Timeout = 5000)]
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

    [Fact(Timeout = 5000)]
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

    [Fact(Timeout = 5000)]
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

    [Fact(Timeout = 5000)]
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

    [Fact(Timeout = 5000)]
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

    [Fact(Timeout = 5000)]
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

