using Signal.Bot.Types;

namespace Signal.Bot.Requests;

public record GetContactsRequest(string Number)
    : RequestBase<ICollection<Contact>?>($"v1/contacts/{Number}", HttpMethod.Get);