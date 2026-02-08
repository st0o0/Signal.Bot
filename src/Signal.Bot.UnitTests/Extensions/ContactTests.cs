using NSubstitute;

namespace Signal.Bot.UnitTests.Extensions;

public class ContactTests : BotTestBase
{
    [Fact(Timeout = 5000)]
    public async Task GetContactsAsync_CallsHttpClient()
    {
        SetupJsonResponse("[]");

        _ = await Client.GetContactsAsync(cancellationToken: TestContext.Current.CancellationToken);

        await HttpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task UpdateContactAsync_CallsHttpClient()
    {
        SetupResponse();

        await Client.UpdateContactAsync("+199", cancellationToken: TestContext.Current.CancellationToken);

        await HttpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task SyncContactsAsync_CallsHttpClient()
    {
        SetupResponse();

        await Client.SyncContactsAsync(cancellationToken: TestContext.Current.CancellationToken);

        await HttpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task GetContactAsync_CallsHttpClient()
    {
        SetupJsonResponse();

        _ = await Client.GetContactAsync("contact-id", cancellationToken: TestContext.Current.CancellationToken);

        await HttpClientMock.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());
    }
}

