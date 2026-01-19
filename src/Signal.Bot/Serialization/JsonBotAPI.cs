using System.Text.Json;

namespace Signal.Bot.Serialization;

public static class JsonBotAPI
{
    public static JsonSerializerOptions Options { get; }

    static JsonBotAPI() => Configure(Options = new JsonSerializerOptions());

    public static void Configure(JsonSerializerOptions options)
    {
        options.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
        options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
    }
}