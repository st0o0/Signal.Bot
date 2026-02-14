using Signal.Bot.Types;

namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to retrieve all contacts stored in the Signal account.
/// </summary>
/// <param name="Number">The phone number of the Signal account whose contacts should be retrieved.</param>
public record GetContactsRequest(string Number)
    : RequestBase<List<Contact>?>($"v1/contacts/{Number}", HttpMethod.Get);