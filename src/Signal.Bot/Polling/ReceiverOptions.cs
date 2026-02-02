namespace Signal.Bot.Polling;

public sealed class ReceiverOptionsBuilder
{
    private ReceiverOptions _options = new(
        TimeSpan.FromSeconds(30),
        false,
        false,
        false,
        false,
        false,
        100,
        false);

    public ReceiverOptionsBuilder WithTimeout(TimeSpan timeout)
    {
        _options = _options with { Timeout = timeout };
        return this;
    }

    public ReceiverOptionsBuilder WithIgnoreAttachments(bool ignoreAttachments)
    {
        _options = _options with { IgnoreAttachments = ignoreAttachments };
        return this;
    }

    public ReceiverOptionsBuilder WithIgnoreStories(bool ignoreStories)
    {
        _options = _options with { IgnoreStories = ignoreStories };
        return this;
    }

    public ReceiverOptionsBuilder WithIgnoreTyping(bool ignoreTyping = true)
    {
        _options = _options with { IgnoreTyping = ignoreTyping };
        return this;
    }

    public ReceiverOptionsBuilder WithIgnoreReceipt(bool ignoreReceipt = true)
    {
        _options = _options with { IgnoreReceipt = ignoreReceipt };
        return this;
    }

    public ReceiverOptionsBuilder WithIgnoreSync(bool ignoreSync = true)
    {
        _options = _options with { IgnoreSync = ignoreSync };
        return this;
    }

    public ReceiverOptionsBuilder WithMaxMessages(int maxMessages)
    {
        _options = _options with { MaxMessages = maxMessages };
        return this;
    }

    public ReceiverOptionsBuilder WithReadReceipts(bool readReceipts)
    {
        _options = _options with { SendReadReceipts = readReceipts };
        return this;
    }

    internal ReceiverOptions Build()
    {
        return _options;
    }
}

public record ReceiverOptions(
    TimeSpan Timeout,
    bool IgnoreAttachments,
    bool IgnoreStories,
    bool IgnoreTyping,
    bool IgnoreReceipt,
    bool IgnoreSync,
    int MaxMessages,
    bool SendReadReceipts);