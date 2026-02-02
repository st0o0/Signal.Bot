namespace Signal.Bot.Requests;

public record SetConfigurationRequest() : RequestBase("v1/configuration")
{
    [JsonPropertyName("logging")] public Logging? Logging { get; set; }
}

public class Logging
{
    [JsonPropertyName("Level")] public string? Level { get; set; }
}