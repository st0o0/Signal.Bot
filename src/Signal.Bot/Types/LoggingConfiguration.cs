using System.Text.Json.Serialization;

namespace Signal.Bot.Types;

public class LoggingConfiguration
{
    [JsonPropertyName("Level")] public string? Level { get; set; }
}