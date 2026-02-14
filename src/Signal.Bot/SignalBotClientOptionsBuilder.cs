using System.Text.RegularExpressions;

namespace Signal.Bot;

/// <summary>
/// Provides a fluent interface for constructing <see cref="SignalBotClientOptions"/> with validation.
/// </summary>
public partial class SignalBotClientOptionsBuilder
{
    private SignalBotClientOptions _options;

    private SignalBotClientOptionsBuilder(string number, string baseUrl)
    {
        _options = new SignalBotClientOptions(number, baseUrl);
    }

    /// <summary>
    /// Creates a new instance of the <see cref="SignalBotClientOptionsBuilder"/>.
    /// </summary>
    /// <returns>A new <see cref="SignalBotClientOptionsBuilder"/> instance.</returns>
    public static SignalBotClientOptionsBuilder Create()
    {
        return new SignalBotClientOptionsBuilder(string.Empty, string.Empty);
    }

    /// <summary>
    /// Sets the base URL of the Signal Bot API endpoint.
    /// </summary>
    /// <param name="baseUrl">The base URL (e.g., "https://signal-api.example.com").</param>
    /// <returns>The builder instance for method chaining.</returns>
    public SignalBotClientOptionsBuilder WithBaseUrl(string baseUrl)
    {
        _options = _options with { BaseUrl = baseUrl };
        return this;
    }

    /// <summary>
    /// Sets the Signal phone number associated with the bot.
    /// </summary>
    /// <param name="number">The phone number in international format (e.g., "+1234567890").</param>
    /// <returns>The builder instance for method chaining.</returns>
    public SignalBotClientOptionsBuilder WithNumber(string number)
    {
        ValidatePhoneNumber(number);
        _options = _options with { Number = number };
        return this;
    }

    /// <summary>
    /// Sets a custom <see cref="System.Net.Http.HttpClient"/> instance for API requests.
    /// If not provided, a default HttpClient will be created with the configured base URL.
    /// </summary>
    /// <param name="httpClient">The HttpClient instance to use.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public SignalBotClientOptionsBuilder WithHttpClient(HttpClient httpClient)
    {
        _options = _options with { HttpClient = httpClient };
        return this;
    }

    /// <summary>
    /// Builds and validates the <see cref="SignalBotClientOptions"/> instance.
    /// Creates a default HttpClient if one was not provided.
    /// </summary>
    /// <returns>The configured and validated <see cref="SignalBotClientOptions"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when Number or BaseUrl is null or whitespace.</exception>
    internal SignalBotClientOptions Build()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(_options.Number);
        ArgumentException.ThrowIfNullOrWhiteSpace(_options.BaseUrl);
        ValidatePhoneNumber(_options.Number);
        if (_options.HttpClient is not null)
        {
            return _options;
        }

        return _options with { HttpClient = new HttpClient { BaseAddress = new Uri(_options.BaseUrl) } };
    }

    private static void ValidatePhoneNumber(string number)
    {
        if (!E164Regex().IsMatch(number))
        {
            throw new ArgumentException(
                "Phone number must be in E.164 format: '+' followed by 1-15 digits (e.g., +1234567890). " +
                "No spaces, dashes, or leading zeros after country code allowed.",
                nameof(number));
        }
    }

    [GeneratedRegex(@"^\+[1-9]\d{0,14}$")]
    private static partial Regex E164Regex();
}