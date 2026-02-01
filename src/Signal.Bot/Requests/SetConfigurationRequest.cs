namespace Signal.Bot.Requests;

public record SetConfigurationRequest() : RequestBase("v1/configuration")
{
    public string? Logging { get; set; }
}