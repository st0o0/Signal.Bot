using Signal.Bot.Types;

namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to receive incoming Signal messages from the queue.
/// </summary>
/// <param name="Number">The phone number of the Signal account receiving messages.</param>
public record GetReceivedMessagesRequest(string Number)
    : RequestBase<ReceivedMessageEnvelope>($"v1/receive/{Number}", HttpMethod.Get);