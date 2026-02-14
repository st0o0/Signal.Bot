using Signal.Bot.Types;

namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to retrieve all known identity keys for contacts, used for verifying end-to-end encryption.
/// </summary>
/// <param name="Number">The phone number of the Signal account whose identity keys should be retrieved.</param>
public record GetIdentitiesRequest(string Number)
    : RequestBase<List<Identity>?>($"v1/identities/{Number}", HttpMethod.Get);