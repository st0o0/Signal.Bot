namespace Signal.Bot.Types;

public class Contact
{
    [JsonPropertyName("number")] public string? Number { get; set; }

    [JsonPropertyName("uuid")] public string? Uuid { get; set; }

    [JsonPropertyName("name")] public string? Name { get; set; }

    [JsonPropertyName("profile_name")] public string? ProfileName { get; set; }

    [JsonPropertyName("username")] public string? Username { get; set; }

    [JsonPropertyName("color")] public string? Color { get; set; }

    [JsonPropertyName("blocked")] public bool Blocked { get; set; }

    [JsonPropertyName("message_expiration")]
    public string? MessageExpiration { get; set; }

    [JsonPropertyName("note")] public string? Note { get; set; }

    [JsonPropertyName("profile")] public Profile? Profile { get; set; }

    [JsonPropertyName("given_name")] public string? GivenName { get; set; }

    [JsonPropertyName("nickname")] public Nickname? Nickname { get; set; }
}