namespace Signal.Bot.Requests;

public record SyncContactsRequest(string Number) : RequestBase($"v1/contacts/{Number}/sync");