namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to retrieve the avatar image of a contact.
/// </summary>
/// <param name="Number">The phone number of the Signal account.</param>
/// <param name="ContactUuid">The UUID of the contact.</param>
public record GetContactAvatarRequest(string Number, string ContactUuid)
    : RequestBase($"v1/contacts/{Number}/{ContactUuid}/avatar", HttpMethod.Get);
