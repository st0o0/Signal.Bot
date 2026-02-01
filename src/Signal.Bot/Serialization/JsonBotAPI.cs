using System.Text.Json;

namespace Signal.Bot.Serialization;

public static class JsonBotAPI
{
    public static JsonSerializerOptions Options { get; }

    public static JsonConverter[] Converters { get; }

    static JsonBotAPI()
    {
        Converters = CreateConverters();
        Options = new JsonSerializerOptions();
        Configure(Options, Converters);
    }

    private static void Configure(JsonSerializerOptions options, JsonConverter[] converters)
    {
        options.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
        options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
        foreach (var jsonConverter in converters)
        {
            options.Converters.Add(jsonConverter);
        }
    }

    private static JsonConverter[] CreateConverters()
    {
        return
        [
            new JsonStringEnumMemberConverter(),
            new TimestampConverter()
        ];
    }
}