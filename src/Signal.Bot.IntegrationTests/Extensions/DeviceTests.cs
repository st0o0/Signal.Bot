using System.Net;
using System.Text.Json;
using Signal.Bot.IntegrationTests.Utils;
using Signal.Bot.Serialization;
using Signal.Bot.Types;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Signal.Bot.IntegrationTests.Extensions;

public class DeviceTests : IntegrationTestBase
{
    [Fact(Timeout = 15000)]
    public async Task GetDevices_ShouldReturnList()
    {
        // Arrange
        var devices = new List<Device> { new() { Id = 1, Name = "Phone" } };
        var json = JsonSerializer.Serialize(devices, JsonBotAPI.Options);
        MockServer
            .Given(Request.Create()
                .WithPath(path => path.Contains("/devices"))
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody(json));

        // Act
        var result = await Client.GetDevicesAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(result);
        Assert.Equal(1, result.First().Id);
        Assert.Equal("Phone", result.First().Name);
    }

    [Fact(Timeout = 15000)]
    public async Task AddDevice_ShouldSucceed()
    {
        // Arrange
        MockServer
            .Given(Request.Create()
                .WithPath(path => path.Contains("/devices"))
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK));

        // Act
        await Client.AddDeviceAsync("tsdevice:/?uuid=xxx",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(MockServer.LogEntries);
    }

    [Fact(Timeout = 15000)]
    public async Task RemoveDevice_ShouldSucceed()
    {
        // Arrange
        MockServer
            .Given(Request.Create()
                .WithPath(path => path.Contains("/devices") && path.EndsWith("/1"))
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK));

        // Act
        await Client.RemoveDeviceAsync(1, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(MockServer.LogEntries);
    }

    [Fact(Timeout = 15000)]
    public async Task DeleteLocalData_ShouldSucceed()
    {
        // Arrange
        MockServer
            .Given(Request.Create()
                .WithPath(path => path.Contains("/devices") && path.Contains("/local-data"))
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK));

        // Act
        await Client.DeleteLocalDataAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(MockServer.LogEntries);
    }

    [Fact(Timeout = 15000)]
    public async Task GetRawDeviceLink_ShouldReturnLink()
    {
        // Arrange
        var link = new RawDeviceLink { DeviceLinkUri = "tsdevice:/?uuid=abc&pub_key=xyz" };
        var json = JsonSerializer.Serialize(link, JsonBotAPI.Options);
        MockServer
            .Given(Request.Create()
                .WithPath("/v1/qrcodelink/raw")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody(json));

        // Act
        var result = await Client.GetRawDeviceLinkAsync("test-device",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("tsdevice:/?uuid=abc&pub_key=xyz", result.DeviceLinkUri);
    }
}
