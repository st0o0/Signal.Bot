using System.Net;
using System.Text.Json;
using Signal.Bot.IntegrationTests.Utils;
using Signal.Bot.Serialization;
using Signal.Bot.Types;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Signal.Bot.IntegrationTests.Extensions;

public class StickerAndReceiptIntegrationTests : IntegrationTestBase
{
    [Fact(Timeout = 15000)]
    public async Task GetStickerPacks_ShouldReturnList()
    {
        // Arrange
        var stickerPacks = new List<StickerPack>
        {
            new() { PackId = "pack1" }
        };
        var json = JsonSerializer.Serialize(stickerPacks, JsonBotAPI.Options);
        MockServer
            .Given(Request.Create()
                .WithPath(path => path.Contains("sticker-packs"))
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody(json));

        // Act
        var result = await Client.GetStickerPacksAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(result);
        Assert.Equal("pack1", result.First().PackId);
    }

    [Fact(Timeout = 15000)]
    public async Task AddStickerPack_ShouldSucceed()
    {
        // Arrange
        MockServer
            .Given(Request.Create()
                .WithPath(path => path.Contains("/sticker-packs"))
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK));

        // Act
        await Client.AddStickerPackAsync("packId", "packKey", cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(MockServer.LogEntries);
    }

    [Fact(Timeout = 15000)]
    public async Task SendReceipt_ShouldSucceed()
    {
        // Arrange
        MockServer
            .Given(Request.Create()
                .WithPath($"/v1/receipts/{BotNumber}")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK));

        // Act
        await Client.SendReceiptAsync(RecipientNumber, DateTime.UtcNow, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(MockServer.LogEntries);
    }
}
