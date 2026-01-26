namespace Signal.Bot;

public record SignalBotClientOptions(string Number, string BaseUrl)
{
    public HttpClient? HttpClient { get; set; }
};

public class SignalBotClientOptionsBuilder
{
    private readonly SignalBotClientOptions _options;

    private SignalBotClientOptionsBuilder(string number, string baseUrl)
    {
        _options = new SignalBotClientOptions(number, baseUrl);
    }

    public static SignalBotClientOptionsBuilder Create(string number, string baseUrl)
    {
        return new SignalBotClientOptionsBuilder(number, baseUrl);
    }

    public SignalBotClientOptionsBuilder WithHttpClient(HttpClient httpClient)
    {
        _options.HttpClient = httpClient;
        return this;
    }
}