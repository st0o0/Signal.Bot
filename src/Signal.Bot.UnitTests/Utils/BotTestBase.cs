using System.Net;
using NSubstitute;

namespace Signal.Bot.UnitTests.Utils;

public abstract class BotTestBase
{
    protected readonly HttpClient HttpClientMock;
    protected readonly SignalBotClient Client;

    protected BotTestBase()
    {
        HttpClientMock = Substitute.For<HttpClient>();
        Client = SignalBotClientFactory.CreateForUnitTests(HttpClientMock);
    }

    protected void SetupJsonResponse(string json = "{}", HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        HttpClientMock.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(json)
            }));
    }
    
    protected void SetupResponse(HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        HttpClientMock.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(statusCode)));
    }
}

