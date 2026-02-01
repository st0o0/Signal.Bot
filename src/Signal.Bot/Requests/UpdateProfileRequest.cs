namespace Signal.Bot.Requests;

public record UpdateProfileRequest(string Number) : RequestBase($"v1/profiles/{Number}")
{
    public string? About { get; set; }
    public string? Base64Avatar { get; set; }
    public string? Name { get; set; }
}