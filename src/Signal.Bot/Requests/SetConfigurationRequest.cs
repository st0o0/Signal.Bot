namespace Signal.Bot.Requests;

public class SetConfigurationRequest() : RequestBase("v1/configuration")
{
    public string? Logging { get; set; }
}
