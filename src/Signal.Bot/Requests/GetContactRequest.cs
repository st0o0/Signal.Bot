using Signal.Bot.Types;

namespace Signal.Bot.Requests;

public record GetContactRequest(string Number, string ContactId)
    : RequestBase<Contact>($"v1/contacts/{Number}/{ContactId}", HttpMethod.Get);