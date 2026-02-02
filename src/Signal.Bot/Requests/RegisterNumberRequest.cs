namespace Signal.Bot.Requests;

public record RegisterNumberRequest(string Number) : RequestBase($"v1/register/{Number}")
{
    [JsonPropertyName("captcha")] public string? Captcha { get; set; }
    
    [JsonPropertyName("use_voice")] public bool? UseVoice { get; set; }
}