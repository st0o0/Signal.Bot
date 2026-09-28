using System.Net;
using System.Text.Json;
using Signal.Bot.IntegrationTests.Utils;
using Signal.Bot.Serialization;
using Signal.Bot.Types;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Signal.Bot.IntegrationTests.Extensions;

public class ContactExtendedTests : IntegrationTestBase
{
    [Fact(Timeout = 15000)]
    public async Task SyncContacts_ShouldSucceed()
    {
        // Arrange
        MockServer
            .Given(Request.Create()
                .WithPath(path => path.Contains("/contacts") && path.EndsWith("/sync"))
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK));

        // Act
        await Client.SyncContactsAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(MockServer.LogEntries);
    }

    [Fact(Timeout = 15000)]
    public async Task GetContact_ShouldReturnContact()
    {
        // Arrange
        var contact = new Contact { Number = RecipientNumber, Name = "Test User" };
        var json = JsonSerializer.Serialize(contact, JsonBotAPI.Options);
        MockServer
            .Given(Request.Create()
                .WithPath(path => path.Contains("/contacts") && !path.Contains("/sync") && !path.Contains("/avatar"))
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody(json));

        // Act
        var result = await Client.GetContactAsync(RecipientNumber,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(RecipientNumber, result.Number);
        Assert.Equal("Test User", result.Name);
    }

    [Fact(Timeout = 15000)]
    public async Task GetContactAvatar_ShouldReturnBytes()
    {
        // Arrange
        const string contactUuid = "uuid-123-456";
        var avatarData = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 };
        MockServer
            .Given(Request.Create()
                .WithPath(path => path.Contains("/contacts") && path.Contains(contactUuid) && path.EndsWith("/avatar"))
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithBody(avatarData));

        // Act
        var result = await Client.GetContactAvatarAsync(contactUuid,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(avatarData, result);
    }
}
