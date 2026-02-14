namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to synchronize the local contact list with Signal's servers to update profile information and registration status.
/// </summary>
/// <param name="Number">The phone number of the Signal account whose contacts should be synchronized.</param>
public record SyncContactsRequest(string Number) : RequestBase($"v1/contacts/{Number}/sync");