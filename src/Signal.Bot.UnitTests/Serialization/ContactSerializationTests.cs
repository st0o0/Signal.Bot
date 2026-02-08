using System.Text.Json;
using Signal.Bot.Requests;
using Signal.Bot.Types;

namespace Signal.Bot.UnitTests.Serialization;

public class ContactSerializationTests
{
    [Fact(Timeout = 5000)]
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

    [Fact(Timeout = 5000)]
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
}

