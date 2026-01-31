using System;

namespace Signal.Bot;

public record SignalBotClientOptions(string Number, string BaseUrl)
{
    public HttpClient? HttpClient { get; set; }
};

public class SignalBotClientOptionsBuilder
{
    private SignalBotClientOptions _options;

    private SignalBotClientOptionsBuilder(string number, string baseUrl)
    {
        _options = new SignalBotClientOptions(number, baseUrl);
    }

    public static SignalBotClientOptionsBuilder Create()
    {
        return new SignalBotClientOptionsBuilder(string.Empty, string.Empty);
    }

    public SignalBotClientOptionsBuilder WithBaseUrl(string baseUrl)
    {
        _options = _options with { BaseUrl = baseUrl };
        return this;
    }

    public SignalBotClientOptionsBuilder WithNumber(string number)
    {
        _options = _options with { Number = number };
        return this;
    }

    public SignalBotClientOptionsBuilder WithHttpClient(HttpClient httpClient)
    {
        _options = _options with { HttpClient = httpClient };
        return this;
    }

    internal SignalBotClientOptions Build()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(_options.Number);
        ArgumentException.ThrowIfNullOrWhiteSpace(_options.BaseUrl);
        if(_options.HttpClient is not null)
        {
            return _options;
        } 
        return _options with { HttpClient = new HttpClient { BaseAddress = new Uri(_options.BaseUrl) }};
    }
}