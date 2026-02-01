using Signal.Bot.Types;

namespace Signal.Bot.Requests;

public record GetReceivedMessagesRequest(string Number)
    : RequestBase<ReceivedMessage>($"v1/receive/{Number}", HttpMethod.Get);