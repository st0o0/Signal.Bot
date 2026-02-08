using System.Net;
using System.Text.Json;
using Signal.Bot.IntegrationTests.Utils;
using Signal.Bot.Serialization;
using Signal.Bot.Types;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Signal.Bot.IntegrationTests.Extensions;

public class IdentityAndContactIntegrationTests : IntegrationTestBase
{
    [Fact(Timeout = 15000)]
    public async Task GetIdentities_ShouldReturnList()
    {
        // Arrange
        var identities = new List<Identity>
        {
            new()
            {
                Number = RecipientNumber,
                Status = IdentityStatus.TrustedVerified,
                Added = DateTime.UtcNow
            }
        };
        var json = JsonSerializer.Serialize(identities, JsonBotAPI.Options);
        MockServer
            .Given(Request.Create()
                .WithPath(path => path.Contains("/identities"))
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody(json));

        // Act
        var result = await Client.GetIdentitiesAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(result);
        Assert.Equal(RecipientNumber, result.First().Number);
    }

    [Fact(Timeout = 15000)]
    public async Task GetContacts_ShouldReturnList()
    {
        // Arrange
        var contacts = new List<Contact>
        {
            new()
            {
                Number = RecipientNumber,
                Name = "Test"
            }
        };
        var json = JsonSerializer.Serialize(contacts, JsonBotAPI.Options);
        MockServer
            .Given(Request.Create()
                .WithPath(path => path.Contains("/contacts"))
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody(json));

        // Act
        var result = await Client.GetContactsAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(result);
        Assert.Equal(RecipientNumber, result.First().Number);
    }

    [Fact(Timeout = 15000)]
    public async Task Search_ShouldReturnResults()
    {
        // Arrange
        var results = new List<Search>
        {
            new()
            {
                Number = RecipientNumber,
                Registered = true
            }
        };
        var json = JsonSerializer.Serialize(results, JsonBotAPI.Options);
        MockServer
            .Given(Request.Create()
                .WithPath(path => path.Contains("/search"))
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody(json));

        // Act
        var result = await Client.SearchNumbersAsync(new[] { RecipientNumber }, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(result);
        Assert.True(result.First().Registered);
    }

    [Fact(Timeout = 15000)]
    public async Task UpdateContact_ShouldSucceed()
    {
        // Arrange
        MockServer
            .Given(Request.Create()
                .WithPath($"/v1/contacts/{BotNumber}")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK));

        // Act
        await Client.UpdateContactAsync(RecipientNumber, name: "New Name", cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(MockServer.LogEntries);
    }
}
