using Signal.Bot.Types;

namespace Signal.Bot.Requests;

public class RemoteDeleteRequest(string number) : RequestBase<RemoteDelete>($"v1/remote-delete/{number}")
{
    [JsonIgnore] public string Number => number;
    public string? Recipient { get; set; }
    public int? Timestamp { get; set; }
}