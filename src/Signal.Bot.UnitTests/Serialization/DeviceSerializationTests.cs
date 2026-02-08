using System.Text.Json;
using Signal.Bot.Requests;
using Signal.Bot.Types;

namespace Signal.Bot.UnitTests.Serialization;

public class DeviceSerializationTests
{
    [Fact(Timeout = 5000)]
    public void TestAddDeviceRequestSerializationAndDeserialization()
    {
        // Arrange
        var addDeviceRequest = new AddDeviceRequest("")
        {
            Uri = "TEST123"
        };

        // Act
        var json = JsonSerializer.Serialize(addDeviceRequest);
        var deserializedAddDeviceRequest = JsonSerializer.Deserialize<AddDeviceRequest>(json);

        // Assert
        Assert.NotNull(deserializedAddDeviceRequest);
        Assert.Equal(addDeviceRequest.Uri, deserializedAddDeviceRequest.Uri);
    }

    [Fact(Timeout = 5000)]
    public void TestDeviceSerializationAndDeserialization()
    {
        // Arrange
        var device = new Device
        {
            Name = "My Device",
            Created = DateTimeOffset.FromUnixTimeMilliseconds(2387149324).DateTime,
            LastSeen = DateTimeOffset.FromUnixTimeMilliseconds(239417043928).DateTime
        };

        // Act
        var json = JsonSerializer.Serialize(device);
        var deserializedDevice = JsonSerializer.Deserialize<Device>(json);

        // Assert
        Assert.NotNull(deserializedDevice);
        Assert.Equal(device.Name, deserializedDevice.Name);
        Assert.Equal(device.Created, deserializedDevice.Created);
        Assert.Equal(device.LastSeen, deserializedDevice.LastSeen);
    }
}

