namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to remove the username from the Signal account, reverting to phone number-only identification.
/// </summary>
/// <param name="Number">The phone number of the Signal account whose username should be removed.</param>
public record RemoveUsernameRequest(string Number)
    : RequestBase<object>($"v1/accounts/{Number}/username", HttpMethod.Delete);