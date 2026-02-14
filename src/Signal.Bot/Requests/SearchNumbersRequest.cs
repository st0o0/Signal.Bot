using Signal.Bot.Types;

namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to search for Signal users by their phone numbers to check registration status.
/// </summary>
/// <param name="Number">The phone number of the Signal account performing the search.</param>
public record SearchNumbersRequest(string Number) : RequestBase<List<Search>?>($"v1/search/{Number}", HttpMethod.Get);