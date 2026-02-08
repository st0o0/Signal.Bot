using WireMock.Server;

namespace Signal.Bot.IntegrationTests.Utils;

public abstract class IntegrationTestBase : IAsyncDisposable
{
    protected readonly WireMockServer MockServer;
    protected readonly SignalBotClient Client;
    protected const string BotNumber = "+491701234567";
    protected const string RecipientNumber = "+491709876543";

    protected IntegrationTestBase()
    {
        MockServer = WireMockServer.Start();
        Client = new SignalBotClient(x => x.WithBaseUrl(MockServer.Url!).WithNumber(BotNumber));
    }

    public virtual async ValueTask DisposeAsync()
    {
        MockServer.Stop();
        MockServer.Dispose();
        await ValueTask.CompletedTask;
    }
}