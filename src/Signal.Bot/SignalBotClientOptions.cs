using System;
using System.Text.Json;
using Signal.Bot.Serialization;

namespace Signal.Bot;

public class SignalBotClientOptions(string number, string baseUrl)
{
    public JsonSerializerOptions JsonSerializerOptions { get; } = JsonBotAPI.Options;
    public string BaseUrl { get; } = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
    public string Number { get; } = number ?? throw new ArgumentNullException(nameof(number));
}