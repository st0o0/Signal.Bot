namespace Signal.Bot.UnitTests.Utils;

public static class SignalBotClientFactory
{
    public static SignalBotClient CreateForIntegrationTests(
        string baseUrl,
        string number = "+1234567890")
    {
        return new SignalBotClient(builder =>
            builder.WithBaseUrl(baseUrl)
                .WithNumber(number));
    }

    public static SignalBotClient CreateForUnitTests(
        HttpClient httpClient,
        string number = "+1234567890",
        string baseUrl = "http://localhost:8080")
    {
        return new SignalBotClient(builder =>
            builder.WithNumber(number)
                .WithBaseUrl(baseUrl)
                .WithHttpClient(httpClient));
    }
}
