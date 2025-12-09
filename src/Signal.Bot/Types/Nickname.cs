namespace Signal.Bot.Types;

public class Nickname
{
    [JsonPropertyName("family_name")] public string? FamilyName { get; set; }

    [JsonPropertyName("given_name")] public string? GivenName { get; set; }

    [JsonPropertyName("name")] public string? Name { get; set; }
}