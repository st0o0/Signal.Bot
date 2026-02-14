using Signal.Bot.Types;

namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to retrieve detailed information about a specific contact.
/// </summary>
/// <param name="Number">The phone number of the Signal account making the request.</param>
/// <param name="ContactId">The phone number or unique identifier of the contact to retrieve.</param>
public record GetContactRequest(string Number, string ContactId)
    : RequestBase<Contact>($"v1/contacts/{Number}/{ContactId}", HttpMethod.Get);