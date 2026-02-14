namespace Signal.Bot.UnitTests.Polling;

public class SignalBotReceiverFilterTests
{
    [Fact]
    public void StartReceivingAsync_FiltersReceipt_WhenRequested()
    {
        var optionsBuilder = new ReceiverOptionsBuilder();
        optionsBuilder.WithIgnoreReceipt();
        var options = optionsBuilder.Build();

        Assert.True(options.IgnoreReceipt);
    }
}