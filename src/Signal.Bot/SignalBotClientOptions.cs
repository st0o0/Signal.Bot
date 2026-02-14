namespace Signal.Bot;

/// <summary>
/// Represents the configuration options for a <see cref="SignalBotClient"/> instance.
/// </summary>
/// <param name="Number">The Signal phone number in international format associated with the bot.</param>
/// <param name="BaseUrl">The base URL of the Signal Bot API endpoint.</param>
public record SignalBotClientOptions(string Number, string BaseUrl)
{
    /// <summary>
    /// Gets or sets the <see cref="System.Net.Http.HttpClient"/> instance to use for API requests.
    /// If not provided, a default HttpClient will be created automatically.
    /// </summary>
    public HttpClient? HttpClient { get; set; }
};