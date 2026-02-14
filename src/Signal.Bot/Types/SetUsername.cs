using System.Text.Json.Serialization;

namespace Signal.Bot.Types;

public class SetUsername
{
    [JsonPropertyName("username")] public string? Username { get; set; }
    [JsonPropertyName("username_link")] public string? UsernameLink { get; set; }
}