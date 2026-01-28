using System;

namespace Signal.Bot.Polling;

public sealed class ReceiverOptions
{
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

    public bool IgnoreAttachments { get; set; }

    public bool IgnoreStories { get; set; }

    public int MaxMessage { get; set; } = 100;

    public bool SendReadReceipts { get; set; } = true;

    public int QueueCapacity { get; set; } = 100;
}