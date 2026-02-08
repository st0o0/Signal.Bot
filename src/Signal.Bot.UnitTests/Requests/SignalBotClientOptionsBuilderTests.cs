namespace Signal.Bot.UnitTests.Requests;

public class SignalBotClientOptionsBuilderTests
{
    [Fact(Timeout = 5000)]
    public void Create_ReturnsEmptyBuilder()
    {
        // Act
        var builder = SignalBotClientOptionsBuilder.Create();

        // Assert
        Assert.NotNull(builder);
    }

    [Fact(Timeout = 5000)]
    public void Build_WithMissingNumber_ThrowsArgumentException()
    {
        // Arrange
        var builder = SignalBotClientOptionsBuilder.Create()
            .WithBaseUrl("http://localhost");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => builder.Build());
    }

    [Fact(Timeout = 5000)]
    public void Build_WithMissingBaseUrl_ThrowsArgumentException()
    {
        // Arrange
        var builder = SignalBotClientOptionsBuilder.Create()
            .WithNumber("+123456789");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => builder.Build());
    }

    [Fact(Timeout = 5000)]
    public void Build_WithValidParameters_ReturnsOptions()
    {
        // Arrange
        const string number = "+123456789";
        const string baseUrl = "http://localhost:8080";
        var builder = SignalBotClientOptionsBuilder.Create()
            .WithNumber(number)
            .WithBaseUrl(baseUrl);

        // Act
        var options = builder.Build();

        // Assert
        Assert.Equal(number, options.Number);
        Assert.Equal(baseUrl, options.BaseUrl);
        Assert.NotNull(options.HttpClient);
        Assert.Equal(new Uri(baseUrl), options.HttpClient.BaseAddress);
    }

    [Fact(Timeout = 5000)]
    public void WithHttpClient_SetsHttpClient()
    {
        // Arrange
        using var client = new HttpClient();
        var builder = SignalBotClientOptionsBuilder.Create()
            .WithNumber("+1")
            .WithBaseUrl("http://localhost")
            .WithHttpClient(client);

        // Act
        var options = builder.Build();

        // Assert
        Assert.Same(client, options.HttpClient);
    }
}
