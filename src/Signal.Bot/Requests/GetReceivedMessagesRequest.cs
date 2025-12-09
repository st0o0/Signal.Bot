using Signal.Bot.Types;

namespace Signal.Bot.Requests;

public class GetReceivedMessagesRequest(string number) : RequestBase<ReceivedMessage>($"v1/receive/{number}")
{
    [JsonIgnore] public string Number => number;

    public override HttpMethod HttpMethod => HttpMethod.Get;

    public override HttpContent? ToHttpContent() => null; // GET requests don't have content
}