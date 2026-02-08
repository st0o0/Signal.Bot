using System.Text.Json;
using NSubstitute;
using Signal.Bot.Serialization;
using Signal.Bot.Types;

namespace Signal.Bot.UnitTests.Extensions;

public class DeviceTests : BotTestBase
{
    [Fact(Timeout = 5000)]
    public async Task GetDevicesAsync_ValidRequest_CallsHttpClient()
    {
        // Arrange
        var devices = new List<Device>();
        var json = JsonSerializer.Serialize(devices);

        SetupJsonResponse(json);

        // Act
        var result = await Client.GetDevicesAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        await HttpClientMock
            .Received(1)
            .SendAsync(
                Arg.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get),
                Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task GetDevicesAsync_WithMultipleDevices_ReturnsCollection()
    {
        var devices = new List<Device>
        {
            new() { Name = "Device 1", Created = DateTime.Now.AddMinutes(1000) },
            new() { Name = "Device 2", Created = DateTime.Now.AddMinutes(2000) },
            new() { Name = "Device 3", Created = DateTime.Now.AddMinutes(3000) }
        };
        var json = JsonSerializer.Serialize(devices, JsonBotAPI.Options);

        SetupJsonResponse(json);

        var result = await Client.GetDevicesAsync(cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
    }

    [Fact(Timeout = 5000)]
    public async Task GetDevicesAsync_WithEmptyList_ReturnsEmptyCollection()
    {
        var devices = new List<Device>();
        var json = JsonSerializer.Serialize(devices);

        SetupJsonResponse(json);

        var result = await Client.GetDevicesAsync(cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact(Timeout = 5000)]
    public async Task AddDeviceAsync_ValidUri_CallsHttpClient()
    {
        // Arrange
        SetupResponse();

        // Act
        await Client.AddDeviceAsync("device://uri", cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        await HttpClientMock
            .Received(1)
            .SendAsync(
                Arg.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post),
                Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task UnregisterDeviceAsync_ValidRequest_CallsHttpClient()
    {
        // Arrange
        SetupResponse();

        // Act
        await Client.UnregisterDeviceAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        await HttpClientMock
            .Received(1)
            .SendAsync(
                Arg.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Delete),
                Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task GetQrCodeLinkAsync_CallsHttpClient()
    {
        var json = JsonSerializer.Serialize("link", JsonBotAPI.Options);
        SetupJsonResponse(json);

        _ = await Client.GetQrCodeLinkAsync("device", cancellationToken: TestContext.Current.CancellationToken);

        await HttpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task GetRawDeviceLinkAsync_CallsHttpClient()
    {
        SetupJsonResponse();

        _ = await Client.GetRawDeviceLinkAsync("device", cancellationToken: TestContext.Current.CancellationToken);

        await HttpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }
}
