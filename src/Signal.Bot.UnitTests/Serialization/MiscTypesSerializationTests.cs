using System.Text.Json;
using Signal.Bot.Serialization;
using Signal.Bot.Types;

namespace Signal.Bot.UnitTests.Serialization;

public class MiscTypesSerializationTests
{
    [Fact(Timeout = 5000)]
    public void TestPaymentSerializationAndDeserialization()
    {
        // Arrange
        var payment = new Payment
        {
            Note = "Lunch money",
            Receipt = "receipt-data-base64"
        };

        // Act
        var json = JsonSerializer.Serialize(payment, JsonBotSerializerContext.Default.Payment);
        var deserialized = JsonSerializer.Deserialize(json, JsonBotSerializerContext.Default.Payment);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal("Lunch money", deserialized.Note);
        Assert.Equal("receipt-data-base64", deserialized.Receipt);
    }

    [Fact(Timeout = 5000)]
    public void TestGroupCallUpdateSerializationAndDeserialization()
    {
        // Arrange
        var update = new GroupCallUpdate { EraId = "era-abc-123" };

        // Act
        var json = JsonSerializer.Serialize(update, JsonBotSerializerContext.Default.GroupCallUpdate);
        var deserialized = JsonSerializer.Deserialize(json, JsonBotSerializerContext.Default.GroupCallUpdate);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal("era-abc-123", deserialized.EraId);
    }

    [Fact(Timeout = 5000)]
    public void TestTextStyleSerializationAndDeserialization()
    {
        // Arrange
        var style = new TextStyle { Start = 5, Length = 10, Style = TextStyleType.Bold };

        // Act
        var json = JsonSerializer.Serialize(style, JsonBotSerializerContext.Default.TextStyle);
        var deserialized = JsonSerializer.Deserialize(json, JsonBotSerializerContext.Default.TextStyle);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal(5, deserialized.Start);
        Assert.Equal(10, deserialized.Length);
        Assert.Equal(TextStyleType.Bold, deserialized.Style);
    }

    [Fact(Timeout = 5000)]
    public void TestStickerSerializationAndDeserialization()
    {
        // Arrange
        var sticker = new Sticker { PackId = "pack-123", StickerId = 42 };

        // Act
        var json = JsonSerializer.Serialize(sticker, JsonBotSerializerContext.Default.Sticker);
        var deserialized = JsonSerializer.Deserialize(json, JsonBotSerializerContext.Default.Sticker);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal("pack-123", deserialized.PackId);
        Assert.Equal(42, deserialized.StickerId);
    }
}
