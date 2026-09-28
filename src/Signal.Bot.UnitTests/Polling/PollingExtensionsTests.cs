using Signal.Bot.Types;
using Signal.Bot.UnitTests.Utils;

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
        using var ctsCanceled = new CancellationTokenSource();
        using var cts =
            CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken, ctsCanceled.Token);

        await ctsCanceled.CancelAsync();

        var disposable = await Client.ReceiveAsync(new DummyHandler(), cancellationToken: cts.Token);

        Assert.NotNull(disposable);
        await disposable.DisposeAsync();
    }

    [Fact(Timeout = 5000)]
    public void StartReceiving_WithHandlerAndCancelledToken_DoesNotThrow()
    {
        using var ctsCanceled = new CancellationTokenSource();
        using var cts =
            CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken, ctsCanceled.Token);

        ctsCanceled.Cancel();

        Client.StartReceiving(new DummyHandler(), cancellationToken: cts.Token);

        Assert.True(true);
    }
}