using System.Net;
using Signal.Bot.IntegrationTests.Utils;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Signal.Bot.IntegrationTests.Extensions;

public class AttachmentIntegrationTests : IntegrationTestBase
{
    [Fact(Timeout = 15000)]
    public async Task GetAttachments_ShouldReturnList()
    {
        // Arrange
        var attachmentIds = new[] { "id1", "id2" };
        MockServer
            .Given(Request.Create()
                .WithPath("/v1/attachments")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithBodyAsJson(attachmentIds));

        // Act
        var result = await Client.GetAttachmentsAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains("id1", result);
        Assert.Contains("id2", result);
    }

    [Fact(Timeout = 15000)]
    public async Task GetAttachment_ShouldReturnBytes()
    {
        // Arrange
        var attachmentId = "test-id";
        var content = "test content"u8.ToArray();
        MockServer
            .Given(Request.Create()
                .WithPath($"/v1/attachments/{attachmentId}")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithBody(content));

        // Act
        var result = await Client.GetAttachmentAsync(attachmentId, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(content, result);
    }

    [Fact(Timeout = 15000)]
    public async Task RemoveAttachment_ShouldSucceed()
    {
        // Arrange
        var attachmentId = "test-id";
        MockServer
            .Given(Request.Create()
                .WithPath($"/v1/attachments/{attachmentId}")
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent));

        // Act
        await Client.RemoveAttachmentAsync(attachmentId, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(MockServer.LogEntries);
    }
}
