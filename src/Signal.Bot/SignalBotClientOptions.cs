using System;
using System.Text.Json;
using Signal.Bot.Serialization;

namespace Signal.Bot;

public class SignalBotClientOptions(string number, string baseUrl, Action<JsonSerializerOptions>? configure = null)
{
    public JsonSerializerOptions JsonSerializerOptions => JsonBotAPI.Configure(configure);
    public string BaseUrl { get; } = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
    public string Number { get; } = number ?? throw new ArgumentNullException(nameof(number));
}