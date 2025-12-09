namespace Signal.Bot.Requests;

public class SyncContactsRequest(string number) : RequestBase($"v1/contacts/{number}/sync")
{
    [JsonIgnore] public string Number => number;
}
