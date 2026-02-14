namespace Signal.Bot;

/// <summary>
/// Provides a fluent interface for constructing <see cref="ReceiverOptions"/> with configurable polling behavior.
/// </summary>
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

    /// <summary>
    /// Sets the timeout duration for polling requests.
    /// </summary>
    /// <param name="timeout">The maximum time to wait for new messages during polling.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public ReceiverOptionsBuilder WithTimeout(TimeSpan timeout)
    {
        _options = _options with { Timeout = timeout };
        return this;
    }

    /// <summary>
    /// Sets whether to ignore message attachments during polling.
    /// </summary>
    /// <param name="ignoreAttachments">If true, attachments will not be downloaded or processed.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public ReceiverOptionsBuilder WithIgnoreAttachments(bool ignoreAttachments)
    {
        _options = _options with { IgnoreAttachments = ignoreAttachments };
        return this;
    }

    /// <summary>
    /// Sets whether to ignore Signal stories during polling.
    /// </summary>
    /// <param name="ignoreStories">If true, story updates will not be received or processed.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public ReceiverOptionsBuilder WithIgnoreStories(bool ignoreStories)
    {
        _options = _options with { IgnoreStories = ignoreStories };
        return this;
    }

    /// <summary>
    /// Sets whether to ignore typing indicator messages.
    /// </summary>
    /// <param name="ignoreTyping">If true, typing indicators will not be received or processed. Default is true.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public ReceiverOptionsBuilder WithIgnoreTyping(bool ignoreTyping = true)
    {
        _options = _options with { IgnoreTyping = ignoreTyping };
        return this;
    }

    /// <summary>
    /// Sets whether to ignore read receipt messages.
    /// </summary>
    /// <param name="ignoreReceipt">If true, read receipts will not be received or processed. Default is true.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public ReceiverOptionsBuilder WithIgnoreReceipt(bool ignoreReceipt = true)
    {
        _options = _options with { IgnoreReceipt = ignoreReceipt };
        return this;
    }

    /// <summary>
    /// Sets whether to ignore sync messages from linked devices.
    /// </summary>
    /// <param name="ignoreSync">If true, sync messages from other devices will not be received or processed. Default is true.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public ReceiverOptionsBuilder WithIgnoreSync(bool ignoreSync = true)
    {
        _options = _options with { IgnoreSync = ignoreSync };
        return this;
    }

    /// <summary>
    /// Sets the maximum number of messages to retrieve in a single polling request.
    /// </summary>
    /// <param name="maxMessages">The maximum number of messages per poll. Must be greater than 0.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public ReceiverOptionsBuilder WithMaxMessages(int maxMessages)
    {
        _options = _options with { MaxMessages = maxMessages };
        return this;
    }

    /// <summary>
    /// Sets whether to automatically send read receipts for received messages.
    /// </summary>
    /// <param name="readReceipts">If true, read receipts will be sent automatically when messages are processed.</param>
    /// <returns>The builder instance for method chaining.</returns>
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

/// <summary>
/// Represents the configuration options for the Signal message receiver during polling operations.
/// </summary>
/// <param name="Timeout">The maximum time to wait for new messages during each polling request.</param>
/// <param name="IgnoreAttachments">If true, message attachments will not be downloaded or processed.</param>
/// <param name="IgnoreStories">If true, Signal story updates will not be received or processed.</param>
/// <param name="IgnoreTyping">If true, typing indicator messages will not be received or processed.</param>
/// <param name="IgnoreReceipt">If true, read receipt messages will not be received or processed.</param>
/// <param name="IgnoreSync">If true, sync messages from linked devices will not be received or processed.</param>
/// <param name="MaxMessages">The maximum number of messages to retrieve in a single polling request.</param>
/// <param name="SendReadReceipts">If true, read receipts will be sent automatically when messages are processed.</param>
public record ReceiverOptions(
    TimeSpan Timeout,
    bool IgnoreAttachments,
    bool IgnoreStories,
    bool IgnoreTyping,
    bool IgnoreReceipt,
    bool IgnoreSync,
    int MaxMessages,
    bool SendReadReceipts);