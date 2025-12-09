namespace Signal.Bot.Requests;

public class UpdateProfileRequest(string number) : RequestBase($"v1/profiles/{number}")
{
    [JsonIgnore] public string Number => number;
    public string? About { get; set; }
    public string? Base64Avatar { get; set; }
    public string? Name { get; set; }
}
