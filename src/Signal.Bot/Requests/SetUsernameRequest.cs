using Signal.Bot.Types;

namespace Signal.Bot.Requests;

public record SetUsernameRequest(string Number) : RequestBase<SetUsername>($"v1/accounts/{Number}/username")
{
    public string? Username { get; set; }
}