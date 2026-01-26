using System;

namespace Signal.Bot.Polling;

public sealed class ReceiverOptionsBuilder
{
    private ReceiverOptions _options = new(TimeSpan.FromSeconds(30), false, false, 100, false, 100);

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

    public ReceiverOptionsBuilder WithQueueCapacity(int queueCapacity)
    {
        _options = _options with { QueueCapacity = queueCapacity };
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
    int MaxMessages,
    bool SendReadReceipts,
    int QueueCapacity);