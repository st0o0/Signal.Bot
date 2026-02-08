using Signal.Bot.Polling;
using Signal.Bot.Types;

namespace Signal.Bot.UnitTests.Polling;

public class PollingExtensionsTests : BotTestBase
{
    private sealed class DummyHandler : IReceivedMessageHandler
    {
        public Task HandleAsync(ISignalBotClient client, ReceivedMessage message, CancellationToken cancellationToken)
            => Task.CompletedTask;

        public Task HandleErrorAsync(ISignalBotClient client, Error error, CancellationToken cancellationToken)
            => Task.CompletedTask;
    }

    [Fact(Timeout = 5000)]
    public async Task ReceiveAsync_WithHandlerAndCancelledToken_ReturnsDisposable()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var disposable = await Client.ReceiveAsync(new DummyHandler(), cancellationToken: cts.Token);

        Assert.NotNull(disposable);
        await disposable.DisposeAsync();
    }

    [Fact(Timeout = 5000)]
    public void StartReceiving_WithHandlerAndCancelledToken_DoesNotThrow()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        Client.StartReceiving(new DummyHandler(), cancellationToken: cts.Token);

        Assert.True(true);
    }
}

