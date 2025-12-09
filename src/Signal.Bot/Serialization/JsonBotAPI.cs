using System;
using System.Text.Json;

namespace Signal.Bot.Serialization;

public class JsonBotAPI
{
    public static JsonSerializerOptions Options { get; }

    static JsonBotAPI() => Configure(Options = new JsonSerializerOptions());

    public static JsonSerializerOptions Configure(Action<JsonSerializerOptions>? configure = null)
        => Configure(Options, configure);

    public static JsonSerializerOptions Configure(JsonSerializerOptions options,
        Action<JsonSerializerOptions>? configure = null)
    {
        configure?.Invoke(options);
        options.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
        options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
        return options;
    }
}