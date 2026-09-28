using System.Text.Json;
using Signal.Bot.Serialization;
using Signal.Bot.Types;

namespace Signal.Bot.UnitTests.Serialization;

public class SharedContactSerializationTests
{
    [Fact]
    public void TestSharedContactSerializationAndDeserialization()
    {
        // Arrange
        var contact = new SharedContact
        {
            Name = new ContactName { Given = "Alice", Family = "Smith" },
            Phone = [new ContactPhone { Value = "+49123", Type = "MOBILE", Label = "Cell" }],
            Email = [new ContactEmail { Value = "alice@example.com", Type = "HOME" }],
            Organization = "ACME Corp"
        };

        // Act
        var json = JsonSerializer.Serialize(contact, JsonBotSerializerContext.Default.SharedContact);
        var deserialized = JsonSerializer.Deserialize(json, JsonBotSerializerContext.Default.SharedContact);

        // Assert
        Assert.NotNull(deserialized);
        Assert.NotNull(deserialized.Name);
        Assert.Equal("Alice", deserialized.Name.Given);
        Assert.Equal("Smith", deserialized.Name.Family);
        Assert.Single(deserialized.Phone!);
        Assert.Equal("+49123", deserialized.Phone![0].Value);
        Assert.Single(deserialized.Email!);
        Assert.Equal("ACME Corp", deserialized.Organization);
    }

    [Fact]
    public void TestSharedContactNullSerialization()
    {
        // Arrange
        var contact = new SharedContact();

        // Act
        var json = JsonSerializer.Serialize(contact, JsonBotSerializerContext.Default.SharedContact);
        var deserialized = JsonSerializer.Deserialize(json, JsonBotSerializerContext.Default.SharedContact);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Null(deserialized.Name);
        Assert.Null(deserialized.Phone);
        Assert.Null(deserialized.Email);
        Assert.Null(deserialized.Organization);
    }

    [Fact]
    public void TestRealSharedContactDeserialization()
    {
        // Arrange
        const string json = """
                            {
                              "name": {
                                "given": "Charlie",
                                "family": "Brown",
                                "middle": "M",
                                "prefix": "Dr",
                                "suffix": "Jr",
                                "nickname": "Chuck"
                              },
                              "phone": [
                                { "value": "+1555", "type": "MOBILE", "label": "Cell" }
                              ],
                              "email": [
                                { "value": "charlie@example.com", "type": "HOME", "label": "Personal" }
                              ],
                              "address": [
                                {
                                  "street": "123 Main St",
                                  "city": "Berlin",
                                  "region": "Berlin",
                                  "postcode": "10115",
                                  "country": "DE",
                                  "type": "HOME"
                                }
                              ],
                              "avatar": {
                                "attachment": { "contentType": "image/jpeg", "id": "avatar-001" },
                                "isProfile": true
                              },
                              "organization": "Peanuts Inc"
                            }
                            """;

        // Act
        var result = JsonSerializer.Deserialize(json, JsonBotSerializerContext.Default.SharedContact);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Name);
        Assert.Equal("Charlie", result.Name.Given);
        Assert.Equal("Brown", result.Name.Family);
        Assert.Equal("Chuck", result.Name.Nickname);
        Assert.Single(result.Phone!);
        Assert.Single(result.Email!);
        Assert.Single(result.Address!);
        Assert.Equal("Berlin", result.Address![0].City);
        Assert.NotNull(result.Avatar);
        Assert.True(result.Avatar.IsProfile);
        Assert.Equal("Peanuts Inc", result.Organization);
    }
}
