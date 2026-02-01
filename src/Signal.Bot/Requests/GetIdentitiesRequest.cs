using Signal.Bot.Types;

namespace Signal.Bot.Requests;

public record GetIdentitiesRequest(string Number)
    : RequestBase<ICollection<Identity>?>($"v1/identities/{Number}", HttpMethod.Get);