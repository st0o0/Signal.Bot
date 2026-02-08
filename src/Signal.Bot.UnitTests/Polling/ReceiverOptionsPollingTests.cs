using Signal.Bot.Polling;

namespace Signal.Bot.UnitTests.Polling;

public class ReceiverOptionsPollingTests
{
    [Fact(Timeout = 5000)]
    public void AsQueryParameter_DefaultOptions_ReturnsCorrectQueryString()
    {
        // Arrange
        var options = new ReceiverOptionsBuilder().Build();

        // Act
        var result = options.AsQueryParameter().Build();

        // Assert
        // Default: timeout=30, ignore_attachments=False, ignore_stories=False, max_messages=100, send_read_receipts=False
        Assert.Contains("timeout=30", result);
        Assert.Contains("ignore_attachments=False", result);
        Assert.Contains("max_messages=100", result);
    }

    [Fact(Timeout = 5000)]
    public void AsQueryParameter_CustomOptions_ReturnsCorrectQueryString()
    {
        // Arrange
        var options = new ReceiverOptionsBuilder()
            .WithTimeout(TimeSpan.FromSeconds(10))
            .WithIgnoreAttachments(true)
            .WithMaxMessages(50)
            .WithReadReceipts(true)
            .Build();

        // Act
        var result = options.AsQueryParameter().Build();

        // Assert
        Assert.Contains("timeout=10", result);
        Assert.Contains("ignore_attachments=True", result);
        Assert.Contains("max_messages=50", result);
        Assert.Contains("send_read_receipts=True", result);
    }
}

