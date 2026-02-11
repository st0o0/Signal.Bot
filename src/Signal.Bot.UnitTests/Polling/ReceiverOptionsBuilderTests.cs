using Signal.Bot.Polling;

namespace Signal.Bot.UnitTests.Polling;

public class ReceiverOptionsBuilderTests
{
    [Fact(Timeout = 5000)]
    public void Build_DefaultValues_AreCorrect()
    {
        var builder = new ReceiverOptionsBuilder();
        var options = builder.Build();

        Assert.Equal(TimeSpan.FromSeconds(30), options.Timeout);
        Assert.False(options.IgnoreAttachments);
        Assert.False(options.IgnoreStories);
        Assert.False(options.IgnoreTyping);
        Assert.False(options.IgnoreReceipt);
        Assert.False(options.IgnoreSync);
        Assert.Equal(100, options.MaxMessages);
        Assert.False(options.SendReadReceipts);
    }

    [Fact(Timeout = 5000)]
    public void WithTimeout_SetsTimeout()
    {
        var timeout = TimeSpan.FromMinutes(1);
        var options = new ReceiverOptionsBuilder()
            .WithTimeout(timeout)
            .Build();

        Assert.Equal(timeout, options.Timeout);
    }

    [Fact(Timeout = 5000)]
    public void WithIgnoreFlags_SetsFlags()
    {
        var options = new ReceiverOptionsBuilder()
            .WithIgnoreAttachments(true)
            .WithIgnoreStories(true)
            .WithIgnoreTyping()
            .WithIgnoreReceipt()
            .WithIgnoreSync()
            .Build();

        Assert.True(options.IgnoreAttachments);
        Assert.True(options.IgnoreStories);
        Assert.True(options.IgnoreTyping);
        Assert.True(options.IgnoreReceipt);
        Assert.True(options.IgnoreSync);
    }

    [Fact(Timeout = 5000)]
    public void WithMaxMessages_SetsMaxMessages()
    {
        var options = new ReceiverOptionsBuilder()
            .WithMaxMessages(500)
            .Build();

        Assert.Equal(500, options.MaxMessages);
    }

    [Fact(Timeout = 5000)]
    public void WithReadReceipts_SetsReadReceipts()
    {
        var options = new ReceiverOptionsBuilder()
            .WithReadReceipts(true)
            .Build();

        Assert.True(options.SendReadReceipts);
    }
}

