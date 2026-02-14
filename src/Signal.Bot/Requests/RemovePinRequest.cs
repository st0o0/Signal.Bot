namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to remove the registration lock PIN from the Signal account.
/// </summary>
/// <param name="Number">The phone number of the Signal account whose PIN should be removed.</param>
public record RemovePinRequest(string Number) : RequestBase($"v1/accounts/{Number}/pin", HttpMethod.Delete);