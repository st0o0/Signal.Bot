namespace Signal.Bot.Requests;

public record UpdateProfileRequest(string Number) : RequestBase($"v1/profiles/{Number}")
{
    [JsonPropertyName("about")] public string? About { get; set; }

    [JsonPropertyName("base64_avatar")] public string? Avatar { get; set; }

    [JsonPropertyName("name")] public string? Name { get; set; }
}