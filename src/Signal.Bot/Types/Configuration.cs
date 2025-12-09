namespace Signal.Bot.Types;

public class Configuration
{

    [JsonPropertyName("logging")]
    public LoggingConfiguration? Logging { get; set; } = null;

}