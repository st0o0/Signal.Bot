using NSubstitute;
using Signal.Bot.UnitTests.Utils;

namespace Signal.Bot.UnitTests.Extensions;

public class PollTests : BotTestBase
{
    [Fact(Timeout = 5000)]
    public async Task AddPollAsync_WithNoArguments_CallsHttpClient()
    {
        // Arrange
        SetupResponse();

        // Act
        await Client.AddPollAsync(null, null, null, null, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        await HttpClientMock
            .Received(1)
            .SendAsync(Arg.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post),
                Arg.Any<CancellationToken>());
    }

    [Theory(Timeout = 5000)]
    [InlineData(true)]
    [InlineData(false)]
    public async Task AddPollAsync_WithMultipleSelections_CallsHttpClient(bool allowMultiple)
    {
        // Arrange
        SetupResponse();

        // Act
        await Client.AddPollAsync(allowMultiple, null, null, null,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        await HttpClientMock
            .Received(1)
            .SendAsync(Arg.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post),
                Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task AddPollAsync_WithAnswers_CallsHttpClient()
    {
        // Arrange
        SetupResponse();
        
        // Act
        await Client.AddPollAsync(null, ["abc", "def", "ghi", "jkl"], null, null, cancellationToken: TestContext.Current.CancellationToken);
        
        // Assert
        await HttpClientMock
            .Received(1)
            .SendAsync(Arg.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post),
                Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task AddPollAsync_WithQuestion_CallsHttpClient()
    {
        // Arrange
        SetupResponse();
        
        // Act
        await Client.AddPollAsync(null, null, "What?!", null, cancellationToken: TestContext.Current.CancellationToken);
        
        // Assert
        await HttpClientMock
            .Received(1)
            .SendAsync(Arg.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post),
                Arg.Any<CancellationToken>());
    }
    
    [Fact(Timeout = 5000)]
    public async Task AddPollAsync_WithRecipient_CallsHttpClient()
    {
        // Arrange
        SetupResponse();
        
        // Act
        await Client.AddPollAsync(null, null, null, "123456789", cancellationToken: TestContext.Current.CancellationToken);
        
        // Assert
        await HttpClientMock
            .Received(1)
            .SendAsync(Arg.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post),
                Arg.Any<CancellationToken>());
    }
    
    [Fact(Timeout = 5000)]
    public async Task AddPollAsync_WithFullDataset_CallsHttpClient()
    {
        // Arrange
        SetupResponse();
        
        // Act
        await Client.AddPollAsync(false, ["abc", "def", "ghi", "jkl"], "What?!", "123456789", cancellationToken: TestContext.Current.CancellationToken);
        
        // Assert
        await HttpClientMock
            .Received(1)
            .SendAsync(Arg.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post),
                Arg.Any<CancellationToken>());
    }
    
    [Fact(Timeout = 5000)]
    public async Task ClosePollAsync_CallsHttpClient()
    {
        // Arrange
        SetupResponse();
        
        // Act
        await Client.ClosePollAsync(DateTime.Now, "123456789", cancellationToken: TestContext.Current.CancellationToken);
        
        // Assert
        await HttpClientMock
            .Received(1)
            .SendAsync(Arg.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Delete),
                Arg.Any<CancellationToken>());
    }
    
    [Fact(Timeout = 5000)]
    public async Task VotePollAsync_WithRequiredArguments_CallsHttpClient()
    {
        // Arrange
        SetupResponse();
        
        // Act
        await Client.VotePollAsync("987654321", DateTime.Now, "123456789", null, cancellationToken: TestContext.Current.CancellationToken);
        
        // Assert
        await HttpClientMock
            .Received(1)
            .SendAsync(Arg.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post),
                Arg.Any<CancellationToken>());
    }
    
    [Fact(Timeout = 5000)]
    public async Task VotePollAsync_WithSingleSelectedAnswer_CallsHttpClient()
    {
        // Arrange
        SetupResponse();
        
        // Act
        await Client.VotePollAsync("987654321", DateTime.Now, "123456789", [1], cancellationToken: TestContext.Current.CancellationToken);
        
        // Assert
        await HttpClientMock
            .Received(1)
            .SendAsync(Arg.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post),
                Arg.Any<CancellationToken>());
    }
    
    [Fact(Timeout = 5000)]
    public async Task VotePollAsync_WithMultipleSelectedAnswers_CallsHttpClient()
    {
        // Arrange
        SetupResponse();
        
        // Act
        await Client.VotePollAsync("987654321", DateTime.Now, "123456789", [1, 3], cancellationToken: TestContext.Current.CancellationToken);
        
        // Assert
        await HttpClientMock
            .Received(1)
            .SendAsync(Arg.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post),
                Arg.Any<CancellationToken>());
    }
}