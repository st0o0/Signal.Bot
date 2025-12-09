namespace Signal.Bot.Requests;

public class SetUsername
{
    [JsonPropertyName("username")] public string? Username { get; set; } = null;

    [JsonPropertyName("username_link")] public string? UsernameLink { get; set; } = null;
}